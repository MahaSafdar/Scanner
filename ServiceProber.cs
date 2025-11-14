using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace gradproject
{
    public class PacketInfo
    {
        public string SourceIP { get; set; } = string.Empty;
        public string DestIP { get; set; } = string.Empty;
        public int SourcePort { get; set; }
        public int DestPort { get; set; }
        public string Banner { get; set; } = string.Empty;
        public Dictionary<string, string> TCPFlags { get; set; } = new();
        public int TTL { get; set; }
    }

    public class ServiceProber
    {
        private readonly List<ServiceProbe> _probes = new();
        private readonly Dictionary<int, string> _knownServices = new();
        private const int ProbeTimeout = 5000;
        private readonly bool _debugMode;
        private readonly ILogger? _logger;
        private readonly ConcurrentBag<PacketInfo> _capturedPackets;

        public class ServiceMatch
        {
            public string ServiceName { get; set; } = string.Empty;
            public string Version { get; set; } = string.Empty;
            public string Banner { get; set; } = string.Empty;
            public string ExtraInfo { get; set; } = string.Empty;
            public double Confidence { get; set; }
        }

        public ServiceProber(string probeFilePath, string servicesFilePath, bool enableDebug = false, ILogger? logger = null, ConcurrentBag<PacketInfo>? capturedPackets = null)
        {
            _debugMode = enableDebug;
            _logger = logger;
            _capturedPackets = capturedPackets ?? new ConcurrentBag<PacketInfo>();
            LoadProbes(probeFilePath);
            LoadServices(servicesFilePath);
        }

        private void LoadServices(string filePath)
        {
            try
            {
                string fullPath = Path.ChangeExtension(filePath, ".txt");
                if (!File.Exists(fullPath))
                {
                    LogDebug($"Warning: Services file not found at {fullPath}");
                    return;
                }

                foreach (var line in File.ReadAllLines(fullPath))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        var serviceParts = parts[1].Split('/');
                        if (serviceParts.Length >= 2 && int.TryParse(serviceParts[0], out int port))
                        {
                            _knownServices[port] = parts[0];
                        }
                    }
                }

                LogDebug($"Loaded {_knownServices.Count} known services");
            }
            catch (Exception ex)
            {
                LogDebug($"Error loading services file: {ex.Message}");
            }
        }

        private void LoadProbes(string filePath)
        {
            try
            {
                string fullPath = Path.ChangeExtension(filePath, ".txt");
                if (!File.Exists(fullPath))
                {
                    LogDebug($"Warning: Probe file not found at {fullPath}");
                    return;
                }

                ServiceProbe? currentProbe = null;
                foreach (var line in File.ReadAllLines(fullPath))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    if (line.StartsWith("Probe"))
                    {
                        if (currentProbe != null)
                            _probes.Add(currentProbe);
                        currentProbe = ServiceProbe.TryCreate(line);
                    }
                    else if (line.StartsWith("match ") && currentProbe != null)
                    {
                        currentProbe.AddMatch(line);
                    }
                    else if (line.StartsWith("softmatch ") && currentProbe != null)
                    {
                        currentProbe.AddSoftMatch(line);
                    }
                }

                if (currentProbe != null)
                    _probes.Add(currentProbe);

                LogDebug($"Loaded {_probes.Count} probes");
            }
            catch (Exception ex)
            {
                LogDebug($"Error loading probe file: {ex.Message}");
            }
        }

        public async Task<ServiceMatch> ProbeServiceAsync(string host, int port, CancellationToken cancellationToken)
        {
            var result = new ServiceMatch();

            // Check known Windows services first
            switch (port)
            {
                case 135:
                    return new ServiceMatch { ServiceName = "Microsoft Windows RPC", Confidence = 1.0 };
                case 139:
                    return new ServiceMatch { ServiceName = "Microsoft Windows netbios-ssn", Confidence = 1.0 };
                case 445:
                    return new ServiceMatch { ServiceName = "Microsoft Windows microsoft-ds", Confidence = 1.0 };
            }

            // Check captured packets for this port
            var packetInfo = _capturedPackets.FirstOrDefault(p =>
                (p.DestPort == port || p.SourcePort == port) &&
                !string.IsNullOrEmpty(p.Banner));

            if (packetInfo != null)
            {
                var bannerMatch = await ProcessBannerAsync(packetInfo.Banner, port);
                if (bannerMatch.Confidence > result.Confidence)
                {
                    result = bannerMatch;
                }
            }

            // Check known services
            if (_knownServices.TryGetValue(port, out string? knownService))
            {
                result.ServiceName = knownService;
                result.Confidence = Math.Max(result.Confidence, 0.7);
            }

            try
            {
                foreach (var probe in _probes)
                {
                    try
                    {
                        using var client = new TcpClient();
                        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        cts.CancelAfter(ProbeTimeout);

                        LogDebug($"Trying probe {probe.Name} on {host}:{port}");
                        await client.ConnectAsync(host, port, cts.Token);

                        using var stream = client.GetStream();
                        stream.ReadTimeout = ProbeTimeout;
                        stream.WriteTimeout = ProbeTimeout;

                        byte[] probeData = Encoding.ASCII.GetBytes(probe.ProbeString);
                        await stream.WriteAsync(probeData, 0, probeData.Length, cts.Token);

                        byte[] buffer = new byte[4096];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        var probeMatch = await ProcessResponseAsync(response, probe, port);
                        if (probeMatch.Confidence > result.Confidence)
                        {
                            result = probeMatch;
                            if (result.Confidence >= 0.9)
                            {
                                return result;
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        LogDebug($"Probe timeout for {host}:{port}");
                    }
                    catch (Exception ex)
                    {
                        LogDebug($"Probe error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogDebug($"Service probe error: {ex.Message}");
            }

            return result;
        }

        private async Task<ServiceMatch> ProcessResponseAsync(string response, ServiceProbe probe, int port)
        {
            var result = new ServiceMatch { Confidence = 0.5 };

            foreach (var match in probe.Matches.Concat(probe.SoftMatches))
            {
                if (Regex.IsMatch(response, match.Pattern, RegexOptions.Singleline))
                {
                    var (service, version, info) = ExtractServiceInfo(match.ServiceInfo, response);

                    if (string.IsNullOrEmpty(version))
                    {
                        version = ExtractVersionFromResponse(response);
                    }

                    result.ServiceName = service;
                    result.Version = version;
                    result.ExtraInfo = info;
                    result.Banner = response;
                    result.Confidence = probe.Matches.Contains(match) ? 0.9 : 0.8;

                    return result;
                }
            }

            return result;
        }

        private async Task<ServiceMatch> ProcessBannerAsync(string banner, int port)
        {
            var result = new ServiceMatch { Confidence = 0.6 };

            try
            {
                var versionPatterns = new[]
                {
                    @"(?:version|ver|v)[:\s]+([0-9][0-9a-zA-Z.-]+)",
                    @"([0-9]+\.[0-9]+\.[0-9]+)",
                    @"([0-9]+\.[0-9]+)",
                    @"([0-9]+\.[0-9a-zA-Z.-]+)"
                };

                foreach (var pattern in versionPatterns)
                {
                    var match = Regex.Match(banner, pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        result.Version = match.Groups[1].Value;
                        break;
                    }
                }

                var serviceMatch = Regex.Match(banner, @"^([a-zA-Z0-9_-]+)(?:\/| |$)");
                if (serviceMatch.Success)
                {
                    result.ServiceName = serviceMatch.Groups[1].Value;
                    result.Confidence = 0.7;
                }

                result.Banner = banner;
            }
            catch (Exception ex)
            {
                LogDebug($"Error processing banner: {ex.Message}");
            }

            return result;
        }

        private (string service, string version, string info) ExtractServiceInfo(string pattern, string response)
        {
            string service = pattern;
            string version = string.Empty;
            string info = string.Empty;

            try
            {
                // Extract version information
                var versionMatch = Regex.Match(response, @"(?:version|ver|v)[:\s]+([0-9][0-9a-zA-Z.-]+)", RegexOptions.IgnoreCase);
                if (versionMatch.Success)
                {
                    version = versionMatch.Groups[1].Value;
                }

                // Extract additional information
                var infoMatch = Regex.Match(response, @"\((.*?)\)|\[(.*?)\]");
                if (infoMatch.Success)
                {
                    info = infoMatch.Groups[1].Value;
                }

                // Format service name
                service = pattern.Split('/')[0].Trim();
            }
            catch (Exception ex)
            {
                LogDebug($"Error extracting service info: {ex.Message}");
            }

            return (service, version, info);
        }

        private string ExtractVersionFromResponse(string response)
        {
            try
            {
                var versionPatterns = new[]
                {
                    @"(?:version|ver|v)[:\s]+([0-9][0-9a-zA-Z.-]+)",
                    @"([0-9]+\.[0-9]+\.[0-9]+)",
                    @"([0-9]+\.[0-9]+)",
                    @"\/([0-9]+\.[0-9a-zA-Z.-]+)"
                };

                foreach (var pattern in versionPatterns)
                {
                    var match = Regex.Match(response, pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        return CleanVersionString(match.Groups[1].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                LogDebug($"Error extracting version: {ex.Message}");
            }

            return string.Empty;
        }

        private string CleanVersionString(string version)
        {
            if (string.IsNullOrEmpty(version))
                return string.Empty;

            try
            {
                version = version.Replace("\0", "")
                               .Replace("\r", "")
                               .Replace("\n", " ")
                               .Replace("\t", " ")
                               .Trim();

                version = Regex.Replace(version, @"\s+", " ");
                version = Regex.Replace(version, @"^\W+|\W+$", "");

                if (version.Length > 50)
                {
                    version = version.Substring(0, 47) + "...";
                }

                return string.IsNullOrWhiteSpace(version) ? string.Empty : version.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        private void LogDebug(string message)
        {
            if (_debugMode)
            {
                _logger?.LogDebug(message);
                Console.WriteLine($"Debug: {message}");
            }
        }
    }

    public class ServiceProbe
    {
        public string Name { get; private set; }
        public string ProbeString { get; private set; }
        public List<ProbeMatch> Matches { get; private set; } = new List<ProbeMatch>();
        public List<ProbeMatch> SoftMatches { get; private set; } = new List<ProbeMatch>();

        private ServiceProbe(string name, string probeString)
        {
            Name = name;
            ProbeString = probeString;
        }

        public static ServiceProbe? TryCreate(string probeLine)
        {
            var parts = probeLine.Split(new[] { ' ' }, 3);
            if (parts.Length < 3)
                return null;

            string name = parts[1];
            string probeString = parts[2].Trim('"');
            return new ServiceProbe(name, probeString);
        }

        public void AddMatch(string matchLine)
        {
            var match = ProbeMatch.TryCreate(matchLine);
            if (match != null)
                Matches.Add(match);
        }

        public void AddSoftMatch(string softMatchLine)
        {
            var softMatch = ProbeMatch.TryCreate(softMatchLine);
            if (softMatch != null)
                SoftMatches.Add(softMatch);
        }
    }

    public class ProbeMatch
    {
        public string Pattern { get; private set; }
        public string ServiceInfo { get; private set; }

        private ProbeMatch(string pattern, string serviceInfo)
        {
            Pattern = pattern;
            ServiceInfo = serviceInfo;
        }

        public static ProbeMatch? TryCreate(string matchLine)
        {
            var parts = matchLine.Split(new[] { ' ' }, 3);
            if (parts.Length < 2)
                return null;

            string pattern = parts[1];
            string serviceInfo = parts.Length > 2 ? parts[2] : "Unknown";
            return new ProbeMatch(pattern, serviceInfo);
        }
    }
}