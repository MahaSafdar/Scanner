using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanner.Scanners
{
    public class RegisteredPortHandler
    {
        private readonly Dictionary<int, ServiceInfo> _registeredServices;
        private readonly string _nmapServicesPath;

        public class ServiceInfo
        {
            public string ServiceName { get; set; }
            public string Protocol { get; set; }
            public double Frequency { get; set; }
            public string Description { get; set; }
        }

        public RegisteredPortHandler(string nmapDataPath)
        {
            _registeredServices = new Dictionary<int, ServiceInfo>();
            _nmapServicesPath = Path.Combine(nmapDataPath, "nmap-services");
            LoadServices();
        }

        private void LoadServices()
        {
            if (!File.Exists(_nmapServicesPath))
            {
                Console.WriteLine("Warning: nmap-services file not found. Registered port detection will be limited.");
                return;
            }

            try
            {
                int loadedServices = 0;
                foreach (string line in File.ReadLines(_nmapServicesPath))
                {
                    if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        string[] portProtocol = parts[1].Split('/');
                        if (portProtocol.Length == 2 && int.TryParse(portProtocol[0], out int port))
                        {
                            double frequency = 0;
                            if (parts.Length > 2)
                            {
                                double.TryParse(parts[2], out frequency);
                            }

                            string description = parts.Length > 3 ? parts[3] : "";

                            _registeredServices[port] = new ServiceInfo
                            {
                                ServiceName = parts[0],
                                Protocol = portProtocol[1],
                                Frequency = frequency,
                                Description = description
                            };
                            loadedServices++;
                        }
                    }
                }
                Console.WriteLine($"Loaded {loadedServices} registered port services from nmap-services");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading nmap-services: {ex.Message}");
            }
        }

        public async Task<string> DetectService(TcpClient client, int port)
        {
            if (!_registeredServices.TryGetValue(port, out var serviceInfo))
            {
                return null;
            }

            try
            {
                using var stream = client.GetStream();

                // Try to get banner first
                string banner = await GetBanner(stream);
                if (!string.IsNullOrEmpty(banner))
                {
                    return $"{serviceInfo.ServiceName} ({banner.Trim()})";
                }

                // Try service-specific probe
                string probeResult = await ProbeService(stream, serviceInfo);
                if (!string.IsNullOrEmpty(probeResult))
                {
                    return $"{serviceInfo.ServiceName} ({probeResult})";
                }

                // Return basic service info if no detailed info available
                return $"{serviceInfo.ServiceName} ({serviceInfo.Protocol})";
            }
            catch
            {
                // If detection fails, return basic service info
                return $"{serviceInfo.ServiceName} ({serviceInfo.Protocol})";
            }
        }

        private async Task<string> GetBanner(NetworkStream stream)
        {
            try
            {
                stream.ReadTimeout = 1500;
                stream.WriteTimeout = 1500;

                byte[] buffer = new byte[2048];
                using var cts = new CancellationTokenSource(1500);

                var readTask = stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                if (await Task.WhenAny(readTask, Task.Delay(1000, cts.Token)) == readTask)
                {
                    int bytesRead = await readTask;
                    if (bytesRead > 0)
                    {
                        return Encoding.ASCII.GetString(buffer, 0, bytesRead)
                            .Replace("\r", "")
                            .Replace("\n", " ")
                            .Trim();
                    }
                }
            }
            catch { }
            return null;
        }

        private async Task<string> ProbeService(NetworkStream stream, ServiceInfo serviceInfo)
        {
            try
            {
                byte[] probe = serviceInfo.Protocol.ToLower() switch
                {
                    "tcp" => Encoding.ASCII.GetBytes("\r\n"),
                    "udp" => new byte[] { 0x0D, 0x0A },
                    _ => Encoding.ASCII.GetBytes("\r\n")
                };

                await stream.WriteAsync(probe, 0, probe.Length);
                return await GetBanner(stream);
            }
            catch { }
            return null;
        }

        public bool IsRegisteredPort(int port)
        {
            return port >= 1024 && port <= 49151;
        }

        public bool HasServiceInfo(int port)
        {
            return _registeredServices.ContainsKey(port);
        }
    }
}