using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using gradproject.models;

namespace gradproject
{
    public class AutomatedScanner : IDisposable
    {
        private readonly ScannerSettings _settings;
        private readonly ILogger _logger;
        private readonly Timer _scheduleCheckTimer;
        private readonly NetworkInterface _networkInterface;
        private bool _isScanning;
        private readonly object _lockObject = new object();
        private readonly string _resultsDirectory;

        public AutomatedScanner(ScannerSettings settings, ILogger logger, NetworkInterface networkInterface)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _networkInterface = networkInterface ?? throw new ArgumentNullException(nameof(networkInterface));
            _resultsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutomatedScanResults");

            // Create results directory if it doesn't exist
            Directory.CreateDirectory(_resultsDirectory);

            // Initialize timer to check schedule every minute
            _scheduleCheckTimer = new Timer(CheckSchedule, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            _logger.LogInformation("AutomatedScanner initialized. Scheduled for {Hour:D2}:{Minute:D2}",
                settings.AutomatedScan.ScheduledHour,
                settings.AutomatedScan.ScheduledMinute);
        }

        private async void CheckSchedule(object? state)
        {
            try
            {
                if (!_settings.AutomatedScan.IsEnabled)
                    return;

                lock (_lockObject)
                {
                    if (_isScanning)
                        return;

                    var now = DateTime.Now;
                    if (now.Hour == _settings.AutomatedScan.ScheduledHour &&
                        now.Minute == _settings.AutomatedScan.ScheduledMinute)
                    {
                        _isScanning = true;
                    }
                    else
                    {
                        return;
                    }
                }

                _logger.LogInformation("Starting scheduled automated scan");
                await RunScan();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in schedule check");
            }
            finally
            {
                _isScanning = false;
            }
        }

        public async Task RunScan()
        {
            if (_isScanning)
            {
                _logger.LogWarning("Scan already in progress");
                return;
            }

            var startTime = DateTime.Now;
            var scanResults = new StringBuilder();

            try
            {
                _isScanning = true;
                _logger.LogInformation("Starting automated scan at {Time}", startTime);

                // Get network information for scanning
                var networkAddress = GetNetworkAddress();
                if (networkAddress == null)
                {
                    _logger.LogError("Could not determine network address");
                    return;
                }

                // Perform configured scans
                if (_settings.AutomatedScan.RunARPScan)
                {
                    var arpResults = await PerformARPScan(networkAddress);
                    scanResults.AppendLine("=== ARP Scan Results ===");
                    scanResults.AppendLine(arpResults);
                    scanResults.AppendLine();
                }

                if (_settings.AutomatedScan.RunICMPScan)
                {
                    var icmpResults = await PerformICMPScan(networkAddress);
                    scanResults.AppendLine("=== ICMP Scan Results ===");
                    scanResults.AppendLine(icmpResults);
                    scanResults.AppendLine();
                }

                if (_settings.AutomatedScan.RunFragmentedICMPScan)
                {
                    var fragResults = await PerformFragmentedICMPScan(networkAddress);
                    scanResults.AppendLine("=== Fragmented ICMP Scan Results ===");
                    scanResults.AppendLine(fragResults);
                    scanResults.AppendLine();
                }

                if (_settings.AutomatedScan.RunPortScan)
                {
                    await PerformPortScan(networkAddress);
                }

                var duration = DateTime.Now - startTime;
                await SaveAutomatedScanResults(scanResults.ToString(), duration);

                _logger.LogInformation("Automated scan completed successfully. Duration: {Duration}", duration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during automated scan");
                scanResults.AppendLine($"Error during scan: {ex.Message}");
            }
            finally
            {
                _isScanning = false;
            }
        }

        private IPAddress? GetNetworkAddress()
        {
            try
            {
                var ipProps = _networkInterface.GetIPProperties();
                var ipv4Address = ipProps.UnicastAddresses
                    .FirstOrDefault(addr => addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                if (ipv4Address == null)
                {
                    _logger.LogError("No IPv4 address found for interface {InterfaceName}", _networkInterface.Name);
                    return null;
                }

                return ipv4Address.Address;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting network address");
                return null;
            }
        }

        private async Task<string> PerformICMPScan(IPAddress networkAddress)
        {
            try
            {
                _logger.LogInformation("Starting automated ICMP scan");
                var startIp = GetNetworkStartAddress(networkAddress);
                var endIp = GetNetworkEndAddress(networkAddress);

                string results = await ICMP.PerformICMPScan(startIp, endIp);
                _logger.LogInformation("ICMP scan completed successfully");
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during automated ICMP scan");
                return $"Error during ICMP scan: {ex.Message}";
            }
        }

        private async Task<string> PerformFragmentedICMPScan(IPAddress networkAddress)
        {
            try
            {
                _logger.LogInformation("Starting automated fragmented ICMP scan");
                var startIp = GetNetworkStartAddress(networkAddress);
                var endIp = GetNetworkEndAddress(networkAddress);

                string results = await ICMPFragmentation.PerformFragmentedICMPScan(startIp, endIp);
                _logger.LogInformation("Fragmented ICMP scan completed successfully");
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during automated fragmented ICMP scan");
                return $"Error during fragmented ICMP scan: {ex.Message}";
            }
        }

        private async Task<string> PerformARPScan(IPAddress networkAddress)
        {
            try
            {
                _logger.LogInformation("Starting automated ARP scan");
                var startIp = GetNetworkStartAddress(networkAddress);
                var endIp = GetNetworkEndAddress(networkAddress);

                string results = await ARP.PerformARPScan(startIp, endIp);
                _logger.LogInformation("ARP scan completed successfully");
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during automated ARP scan");
                return $"Error during ARP scan: {ex.Message}";
            }
        }

        private async Task PerformPortScan(IPAddress networkAddress)
        {
            try
            {
                _logger.LogInformation("Starting automated port scan");
                var config = new ScannerConfiguration
                {
                    MaxConcurrentScans = _settings.Network.MaxConcurrentScans,
                    ConnectionTimeout = _settings.Network.ConnectionTimeout,
                    EnableDebugLogging = _settings.Logging.EnableDebugLogging,
                    RetryAttempts = _settings.Network.RetryAttempts,
                    RetryDelay = _settings.Network.RetryDelay,
                    BatchSize = 1000  // Default batch size for automated scans
                };

                using var scanner = new PortScannerc(config, _logger);
                var prober = new ServiceProber(
                probeFilePath: "nmap-service-probes",
                servicesFilePath: "nmap-services",
                enableDebug: _settings.Logging.EnableDebugLogging,
                logger: _logger);

                var progress = new Progress<string>(status => _logger.LogInformation(status));

                // Scan well-known ports by default for automated scans
                var ports = Enumerable.Range(1, 1024);
                var results = await scanner.ScanAsync(networkAddress.ToString(), ports, prober, progress);

                _logger.LogInformation("Port scan completed. Found {OpenPorts} open ports",
                    results.Count(r => r.IsOpen));

                // Format and save port scan results
                var formattedResults = ScanResultsFormatter.FormatPortScanResults(results);
                await SavePortScanResults(formattedResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during automated port scan");
            }
        }

        private IPAddress GetNetworkStartAddress(IPAddress networkAddress)
        {
            byte[] ipBytes = networkAddress.GetAddressBytes();
            ipBytes[3] = 1; // Start from x.x.x.1
            return new IPAddress(ipBytes);
        }

        private IPAddress GetNetworkEndAddress(IPAddress networkAddress)
        {
            byte[] ipBytes = networkAddress.GetAddressBytes();
            ipBytes[3] = 254; // End at x.x.x.254
            return new IPAddress(ipBytes);
        }

        private async Task SaveAutomatedScanResults(string results, TimeSpan duration)
        {
            try
            {
                var timestamp = DateTime.Now;
                var fileName = $"AutomatedScan_{timestamp:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(_resultsDirectory, fileName);

                var fullResults = new StringBuilder();
                fullResults.AppendLine($"=== Automated Scan Results ===");
                fullResults.AppendLine($"Scan Time: {timestamp}");
                fullResults.AppendLine($"Duration: {duration.TotalSeconds:F2} seconds");
                fullResults.AppendLine("=========================");
                fullResults.AppendLine();
                fullResults.AppendLine(results);

                await File.WriteAllTextAsync(filePath, fullResults.ToString());
                _logger.LogInformation("Automated scan results saved to: {FilePath}", filePath);

                // Save in additional formats if configured
                if (_settings.Logging.SaveReportsAsJson)
                {
                    var jsonPath = Path.ChangeExtension(filePath, ".json");
                    await File.WriteAllTextAsync(jsonPath, System.Text.Json.JsonSerializer.Serialize(
                        new
                        {
                            ScanTime = timestamp,
                            Duration = duration,
                            Results = results
                        },
                        new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
                    ));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving automated scan results");
            }
        }

        private async Task SavePortScanResults(string results)
        {
            try
            {
                var timestamp = DateTime.Now;
                var fileName = $"PortScan_{timestamp:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(_resultsDirectory, fileName);

                await File.WriteAllTextAsync(filePath, results);
                _logger.LogInformation("Port scan results saved to: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving port scan results");
            }
        }

        public void Dispose()
        {
            _scheduleCheckTimer?.Dispose();
            _logger.LogInformation("AutomatedScanner disposed");
        }
    }
}