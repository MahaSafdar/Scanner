using System.Net;
using System.Net.NetworkInformation;
using System.Runtime;
using System.Text;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using gradproject.models;
using SharpPcap;
using System.Collections.Concurrent;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace gradproject
{

    public class Program
    {
        private static ScannerSettings _settings = new();
        private static readonly ScanReport _scanReport = new();
        private static ILogger<Program> _logger = null!;
        private static AutomatedScanner? _automatedScanner;
        private static NetworkInterface? _selectedInterface;
        private static SmartVulnerabilityScanner? _vulnerabilityScanner;
        private static readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static readonly string _resultsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScanResults");

        private static readonly ConcurrentBag<PacketInfo> CapturedPackets = new();
        private static ILiveDevice CaptureDevice;
        static async Task Main(string[] args)
        {
            try
            {
                await InitializeApplication(args);
                await RunMainLoop();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.ResetColor();
                _logger?.LogCritical(ex, "Application crashed");
            }
            finally
            {
                _automatedScanner?.Dispose();
                if (CaptureDevice != null)
                {
                    CaptureDevice.StopCapture();
                    CaptureDevice.Close();
                }
                Log.CloseAndFlush();
            }



        }
        private static void ReportProgress(string message)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(message);
        }

        private static void InitializePacketCapture()
        {
            try
            {
                var devices = LibPcapLiveDeviceList.Instance;
                if (devices.Count == 0)
                {
                    _logger.LogWarning("No capture devices found.");
                    return;
                }

                CaptureDevice = devices.FirstOrDefault(d => d.Interface.FriendlyName?.Contains("Ethernet") ?? false)
                               ?? devices[0];

                CaptureDevice.Open(new DeviceConfiguration
                {
                    Mode = DeviceModes.Promiscuous,
                    ReadTimeout = 1000
                });

                _logger.LogInformation($"Using capture device: {CaptureDevice.Description}");

                CaptureDevice.Filter = "tcp";
                CaptureDevice.OnPacketArrival += Device_OnPacketArrival;
                CaptureDevice.StartCapture();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize packet capture");
            }
        }
        private static async Task InitializeApplication(string[] args)
        {
            try
            {

                Directory.CreateDirectory(_logDirectory);
                Directory.CreateDirectory(_resultsDirectory);

                // Setup logging configuration
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: Path.Combine(_logDirectory, $"scanner-{DateTime.Now:yyyyMMdd}.log"),
                        fileSizeLimitBytes: 10_000_000,
                        rollOnFileSizeLimit: true,
                        shared: true)
                    .CreateLogger();

                // Create logger factory
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.AddSerilog();
                    builder.SetMinimumLevel(LogLevel.Debug);
                });

                _logger = loggerFactory.CreateLogger<Program>();
                _logger.LogInformation("Application starting...");

                // Load settings
                try
                {
                    _settings = await ScannerSettings.LoadAsync();
                    _logger.LogInformation("Settings loaded successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load settings, using defaults");
                    _settings = new ScannerSettings();
                    await _settings.SaveAsync();
                }



                // Handle debug mode
                if (args.Length > 0 && args[0].ToLower() == "--debug")
                {
                    _settings.Logging.EnableDebugLogging = true;
                    await _settings.SaveAsync();
                    _logger.LogInformation("Debug mode enabled");
                }

                // Select network interface
                try
                {
                    _selectedInterface = await NetworkInterfaceManager.SelectNetworkInterfaceAsync();
                    if (_selectedInterface == null)
                    {
                        throw new InvalidOperationException("No valid network interface selected");
                    }
                    _logger.LogInformation("Selected network interface: {InterfaceName}", _selectedInterface.Name);

                    InitializePacketCapture();
                    var prober = new ServiceProber(
                        probeFilePath: "nmap-service-probes",
                        servicesFilePath: "nmap-services",
                        enableDebug: _settings.Logging.EnableDebugLogging,
                        logger: _logger, capturedPackets: CapturedPackets);

                    if (_settings.AutomatedScan.IsEnabled)
                    {
                        _automatedScanner = new AutomatedScanner(_settings, _logger, _selectedInterface);
                        _logger.LogInformation("Automated scanner initialized");
                    }

                    const string nvdApiKey = "ffe7680f-615f-47bc-a111-499bcb02274e";
                    _vulnerabilityScanner = new SmartVulnerabilityScanner(_logger, prober, nvdApiKey);
                    _logger.LogInformation("Vulnerability scanner initialized");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize network components");
                    throw new InvalidOperationException("Failed to initialize network components. Application cannot continue.", ex);
                }

                _scanReport.ScanTime = DateTime.Now;
                _logger.LogInformation("Application initialization completed successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize application");
                throw;
            }
        }

        private static void Device_OnPacketArrival(object sender, PacketCapture e)
        {
            try
            {
                var rawPacket = e.GetPacket();
                var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

                var tcpPacket = packet.Extract<TcpPacket>();
                var ipPacket = packet.Extract<IPPacket>();

                if (tcpPacket != null && ipPacket != null)
                {
                    var flags = tcpPacket.Flags;
                    var packetInfo = new PacketInfo
                    {
                        SourceIP = ipPacket.SourceAddress.ToString(),
                        DestIP = ipPacket.DestinationAddress.ToString(),
                        SourcePort = tcpPacket.SourcePort,
                        DestPort = tcpPacket.DestinationPort,
                        TTL = ipPacket.TimeToLive,
                        TCPFlags = new Dictionary<string, string>
                {
                    { "SYN", ((flags & 0x02) != 0).ToString() },
                    { "ACK", ((flags & 0x10) != 0).ToString() },
                    { "RST", ((flags & 0x04) != 0).ToString() },
                    { "FIN", ((flags & 0x01) != 0).ToString() }
                }
                    };

                    if (tcpPacket.PayloadData?.Length > 0)
                    {
                        packetInfo.Banner = Encoding.ASCII.GetString(tcpPacket.PayloadData)
                            .Replace("\0", "")
                            .Trim();
                    }

                    CapturedPackets.Add(packetInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing captured packet");
            }
        }

        private static async Task RunMainLoop()
        {
            // Initialize ServiceProber with both files
            var prober = new ServiceProber(
                probeFilePath: "nmap-service-probes",
                servicesFilePath: "nmap-services",
                enableDebug: _settings.Logging.EnableDebugLogging,
                logger: _logger, capturedPackets: CapturedPackets);

            while (true)
            {
                try
                {
                    await DisplayMainMenu();
                    string? choice = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(choice))
                    {
                        continue;
                    }

                    if (choice == "8")
                    {
                        _logger.LogInformation("Application exit requested");
                        break;
                    }

                    await ProcessMenuChoice(choice, prober);

                    // No need to display this message as each scan method now has its own "Press any key" prompt
                    // Console.WriteLine("\nPress any key to continue...");
                    // Console.ReadKey(true);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("\nOperation cancelled.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in main loop");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
            }
        }

        private static async Task DisplayMainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════╗");
            Console.WriteLine("║  GRAD PROJECT SCANNER     ║");
            Console.WriteLine("╚═══════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("1. ICMP Scan");
            Console.WriteLine("2. ICMP Scan with Fragmentation");
            Console.WriteLine("3. ARP Scan");
            Console.WriteLine("4. Port Scan");
            Console.WriteLine("5. Vulnerability Scan");
            Console.WriteLine("6. Automated Scan Settings");
            Console.WriteLine("7. Settings");
            Console.WriteLine("8. Exit");

            if (_settings.AutomatedScan.IsEnabled)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nAutomated scan is active (Scheduled for {_settings.AutomatedScan.ScheduledHour:D2}:{_settings.AutomatedScan.ScheduledMinute:D2})");
                Console.ResetColor();
            }

            Console.Write("\nEnter your choice: ");
            await Task.CompletedTask; // Makes the method properly async
        }

        private static async Task ProcessMenuChoice(string choice, ServiceProber prober)
        {
            switch (choice)
            {
                case "1":
                    await PerformICMPScan();
                    break;
                case "2":
                    await PerformFragmentedICMPScan();
                    break;
                case "3":
                    await PerformARPScan();
                    break;
                case "4":
                    await PerformPortScan(prober);
                    break;
                case "5":
                    await PerformVulnerabilityScan();
                    break;
                case "6":
                    await ManageAutomatedScanSettings();
                    break;
                case "7":
                    await ManageSettings();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.ResetColor();
                    break;
            }
        }

        private static async Task PerformICMPScan()
        {
            var (startIP, endIP) = GetIPRange();
            if (startIP == null || endIP == null)
            {
                Console.WriteLine("Invalid IP range specified.");
                return;
            }

            try
            {
                var startTime = DateTime.Now;
                string results = await ICMP.PerformICMPScan(startIP, endIP);
                var duration = DateTime.Now - startTime;
                await SaveScanResults("ICMP Scan", results, duration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ICMP scan");
                Console.WriteLine($"Error during scan: {ex.Message}");
            }
        }

        private static async Task PerformFragmentedICMPScan()
        {
            var (startIP, endIP) = GetIPRange();
            if (startIP == null || endIP == null)
            {
                Console.WriteLine("Invalid IP range specified.");
                return;
            }

            try
            {
                var startTime = DateTime.Now;
                string results = await ICMPFragmentation.PerformFragmentedICMPScan(startIP, endIP);
                var duration = DateTime.Now - startTime;
                await SaveScanResults("Fragmented ICMP Scan", results, duration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during fragmented ICMP scan");
                Console.WriteLine($"Error during scan: {ex.Message}");
            }
        }

        private static async Task PerformARPScan()
        {
            var (startIP, endIP) = GetIPRange();
            if (startIP == null || endIP == null)
            {
                Console.WriteLine("Invalid IP range specified.");
                return;
            }

            try
            {
                var startTime = DateTime.Now;
                Console.WriteLine("\nStarting ARP scan...");
                string results = await ARP.PerformARPScan(startIP, endIP);
                var duration = DateTime.Now - startTime;
                await SaveScanResults("ARP Scan", results, duration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ARP scan");
                Console.WriteLine($"Error during scan: {ex.Message}");
            }
        }

        private static async Task PerformPortScan(ServiceProber prober)
        {
            var (startIP, endIP) = GetIPRange();
            if (startIP == null || endIP == null)
            {
                Console.WriteLine("Invalid IP range specified.");
                return;
            }

            var portRange = SelectPortRange().ToList();
            if (!portRange.Any())
            {
                Console.WriteLine("No valid ports selected.");
                return;
            }

            try
            {
                var scanner = new PortScannerc(
                    new ScannerConfiguration
                    {
                        MaxConcurrentScans = _settings.Network.MaxConcurrentScans,
                        ConnectionTimeout = _settings.Network.ConnectionTimeout,
                        EnableDebugLogging = _settings.Logging.EnableDebugLogging,
                        BatchSize = 1000,
                        RetryAttempts = _settings.Network.RetryAttempts,
                        RetryDelay = _settings.Network.RetryDelay
                    },
                    _logger
                );

                var progress = new Progress<string>(message => Console.WriteLine(message));
                var startTime = DateTime.Now;

                Console.WriteLine("\nStarting port scan...");
                var results = await scanner.ScanAsync(startIP.ToString(), portRange, prober, progress);

                // Format results using the formatter
                var formattedResults = ScanResultsFormatter.FormatPortScanResults(results);

                var duration = DateTime.Now - startTime;
                await SaveScanResults("Port Scan", formattedResults, duration);

                // Add these lines to wait for user input
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nPort scan cancelled.");
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during port scan");
                Console.WriteLine($"Error during scan: {ex.Message}");
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
        }



        private static async Task PerformVulnerabilityScan()
        {
            var (startIP, endIP) = GetIPRange();
            if (startIP == null || endIP == null)
            {
                Console.WriteLine("Invalid IP range specified.");
                return;
            }

            var portRange = SelectPortRange().ToList();
            if (!portRange.Any())
            {
                Console.WriteLine("No valid ports selected.");
                return;
            }

            try
            {
                var progress = new Progress<string>(message => Console.WriteLine(message));
                var startTime = DateTime.Now;

                Console.WriteLine("\nStarting vulnerability scan...");
                if (_vulnerabilityScanner == null)
                {
                    throw new InvalidOperationException("Vulnerability scanner not initialized");
                }

                var results = await _vulnerabilityScanner.ScanTarget(startIP.ToString(), portRange, progress);

                // Format results using the formatter
                var formattedResults = ScanResultsFormatter.FormatVulnerabilityResults(results);

                var duration = DateTime.Now - startTime;
                await SaveScanResults("Vulnerability Scan", formattedResults, duration);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nVulnerability scan cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during vulnerability scan");
                Console.WriteLine($"Error during scan: {ex.Message}");
            }
        }
        private static async Task SaveScanResults(string scanType, string results, TimeSpan duration)
        {
            try
            {
                if (_settings.Logging.SaveReportsAsHtml && _scanReport != null)
                {
                    var htmlPath = Path.Combine(_settings.Logging.LogDirectory,
                        $"{scanType.Replace(" ", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.html");
                    await _scanReport.SaveAsHtmlAsync(htmlPath);
                }

                if (_settings.Logging.SaveReportsAsJson && _scanReport != null)
                {
                    var jsonPath = Path.Combine(_settings.Logging.LogDirectory,
                        $"{scanType.Replace(" ", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                    await _scanReport.SaveAsJsonAsync(jsonPath);
                }

                await SaveScanResultsToFile(scanType, FormatScanResults(scanType, results, duration));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving scan results");
                Console.WriteLine($"Error saving results: {ex.Message}");
            }
        }

        private static string FormatScanResults(string scanType, string results, TimeSpan duration)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== {scanType} Results ===");
            sb.AppendLine($"Scan Time: {DateTime.Now}");
            sb.AppendLine($"Duration: {duration.TotalSeconds:F2} seconds");
            sb.AppendLine("=========================");
            sb.AppendLine();
            sb.AppendLine(results);
            return sb.ToString();
        }

        private static async Task SaveScanResultsToFile(string scanType, string results)
        {
            string fileName = $"{scanType}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(_resultsDirectory, fileName);

            try
            {
                await File.WriteAllTextAsync(filePath, results);
                _logger.LogInformation("Scan results saved to: {FilePath}", filePath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Scan results saved to: {filePath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save scan results");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error saving results: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static (IPAddress? startIP, IPAddress? endIP) GetIPRange()
        {
            Console.Write("Enter the start IP address of the range to scan: ");
            string? startIpString = Console.ReadLine();
            Console.Write("Enter the end IP address of the range to scan: ");
            string? endIpString = Console.ReadLine();

            if (IPAddress.TryParse(startIpString, out IPAddress? startIP) &&
                IPAddress.TryParse(endIpString, out IPAddress? endIP))
            {
                return (startIP, endIP);
            }
            return (null, null);
        }

        private static IEnumerable<int> SelectPortRange()
        {
            Console.WriteLine("\nSelect port scanning option:");
            Console.WriteLine("1. Well-known ports (1-1023)");
            Console.WriteLine("2. Registered ports (1024-49151)");
            Console.WriteLine("3. Dynamic ports (49152-65535)");
            Console.WriteLine("4. All ports (1-65535)");
            Console.WriteLine("5. Custom range");
            Console.Write("\nEnter your choice: ");

            string? portChoice = Console.ReadLine();

            try
            {
                return portChoice switch
                {
                    "1" => Enumerable.Range(1, 1023),
                    "2" => Enumerable.Range(1024, 49151 - 1024 + 1),
                    "3" => Enumerable.Range(49152, 65535 - 49152 + 1),
                    "4" => Enumerable.Range(1, 65535),
                    "5" => GetCustomPortRange(),
                    _ => throw new ArgumentException("Invalid choice. Please try again.")
                };
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error setting port range: {ex.Message}");
                Console.ResetColor();
                return Enumerable.Empty<int>();
            }
        }

        private static IEnumerable<int> GetCustomPortRange()
        {
            Console.Write("Enter the start port: ");
            if (!int.TryParse(Console.ReadLine(), out int startPort) || startPort < 1 || startPort > 65535)
            {
                throw new ArgumentException("Invalid start port. Must be between 1 and 65535.");
            }

            Console.Write("Enter the end port: ");
            if (!int.TryParse(Console.ReadLine(), out int endPort) || endPort < 1 || endPort > 65535)
            {
                throw new ArgumentException("Invalid end port. Must be between 1 and 65535.");
            }

            if (startPort > endPort)
            {
                throw new ArgumentException("Start port cannot be greater than end port.");
            }

            return Enumerable.Range(startPort, endPort - startPort + 1);
        }

        private static async Task ManageSettings()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Settings Menu");
                Console.WriteLine("-----------------");
                Console.WriteLine("1. Network Settings");
                Console.WriteLine("2. Logging Settings");
                Console.WriteLine("3. Save Settings");
                Console.WriteLine("4. Reset to Defaults");
                Console.WriteLine("5. Back to Main Menu");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ManageNetworkSettings();
                            break;
                        case "2":
                            await ManageLoggingSettings();
                            break;
                        case "3":
                            await _settings.SaveAsync();
                            Console.WriteLine("Settings saved successfully.");
                            break;
                        case "4":
                            if (await ConfirmAction("Are you sure you want to reset all settings to defaults?"))
                            {
                                _settings = new ScannerSettings();
                                await _settings.SaveAsync();
                                Console.WriteLine("Settings reset to defaults.");
                            }
                            break;
                        case "5":
                            return;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error managing settings");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }

                await Task.Delay(1000);
            }
        }
        private static async Task ManageNetworkSettings()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Network Settings");
                Console.WriteLine("-----------------");
                Console.WriteLine($"1. Max Concurrent Scans: {_settings.Network.MaxConcurrentScans}");
                Console.WriteLine($"2. Connection Timeout: {_settings.Network.ConnectionTimeout}ms");
                Console.WriteLine($"3. Retry Attempts: {_settings.Network.RetryAttempts}");
                Console.WriteLine($"4. Retry Delay: {_settings.Network.RetryDelay}ms");
                Console.WriteLine($"5. Enable Fragmentation: {_settings.Network.EnableFragmentation}");
                Console.WriteLine("6. Back to Settings Menu");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Enter new value (10-1000): ");
                            if (int.TryParse(Console.ReadLine(), out int maxScans) && maxScans >= 10 && maxScans <= 1000)
                            {
                                _settings.Network.MaxConcurrentScans = maxScans;
                            }
                            else
                            {
                                Console.WriteLine("Invalid value. Must be between 10 and 1000.");
                            }
                            break;

                        case "2":
                            Console.Write("Enter new timeout value in milliseconds (100-30000): ");
                            if (int.TryParse(Console.ReadLine(), out int timeout) && timeout >= 100 && timeout <= 30000)
                            {
                                _settings.Network.ConnectionTimeout = timeout;
                            }
                            else
                            {
                                Console.WriteLine("Invalid value. Must be between 100 and 30000.");
                            }
                            break;

                        case "3":
                            Console.Write("Enter new retry attempts (0-5): ");
                            if (int.TryParse(Console.ReadLine(), out int retries) && retries >= 0 && retries <= 5)
                            {
                                _settings.Network.RetryAttempts = retries;
                            }
                            else
                            {
                                Console.WriteLine("Invalid value. Must be between 0 and 5.");
                            }
                            break;

                        case "4":
                            Console.Write("Enter new retry delay in milliseconds (100-5000): ");
                            if (int.TryParse(Console.ReadLine(), out int delay) && delay >= 100 && delay <= 5000)
                            {
                                _settings.Network.RetryDelay = delay;
                            }
                            else
                            {
                                Console.WriteLine("Invalid value. Must be between 100 and 5000.");
                            }
                            break;

                        case "5":
                            _settings.Network.EnableFragmentation = !_settings.Network.EnableFragmentation;
                            Console.WriteLine($"Fragmentation {(_settings.Network.EnableFragmentation ? "enabled" : "disabled")}.");
                            break;

                        case "6":
                            return;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }

                    await _settings.SaveAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error managing network settings");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }

                await Task.Delay(1000);
            }
        }

        private static async Task ManageLoggingSettings()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Logging Settings");
                Console.WriteLine("-----------------");
                Console.WriteLine($"1. Enable Debug Logging: {_settings.Logging.EnableDebugLogging}");
                Console.WriteLine($"2. Log Directory: {_settings.Logging.LogDirectory}");
                Console.WriteLine($"3. Save Reports as HTML: {_settings.Logging.SaveReportsAsHtml}");
                Console.WriteLine($"4. Save Reports as JSON: {_settings.Logging.SaveReportsAsJson}");
                Console.WriteLine("5. Back to Settings Menu");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            _settings.Logging.EnableDebugLogging = !_settings.Logging.EnableDebugLogging;
                            Console.WriteLine($"Debug logging {(_settings.Logging.EnableDebugLogging ? "enabled" : "disabled")}.");
                            break;

                        case "2":
                            Console.Write("Enter new log directory path: ");
                            string? dir = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(dir))
                            {
                                try
                                {
                                    Directory.CreateDirectory(dir);
                                    _settings.Logging.LogDirectory = dir;
                                }
                                catch (Exception ex)
                                {
                                    throw new ArgumentException($"Invalid directory path: {ex.Message}");
                                }
                            }
                            break;

                        case "3":
                            _settings.Logging.SaveReportsAsHtml = !_settings.Logging.SaveReportsAsHtml;
                            Console.WriteLine($"HTML reports {(_settings.Logging.SaveReportsAsHtml ? "enabled" : "disabled")}.");
                            break;

                        case "4":
                            _settings.Logging.SaveReportsAsJson = !_settings.Logging.SaveReportsAsJson;
                            Console.WriteLine($"JSON reports {(_settings.Logging.SaveReportsAsJson ? "enabled" : "disabled")}.");
                            break;

                        case "5":
                            return;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }

                    await _settings.SaveAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error managing logging settings");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }

                await Task.Delay(1000);
            }
        }

        private static async Task ManageAutomatedScanSettings()
        {
            Console.Clear();
            Console.WriteLine("=== Automated Scan Settings ===\n");

            Console.Write("Enable automated scanning? (y/n): ");
            bool wasEnabled = _settings.AutomatedScan.IsEnabled;
            _settings.AutomatedScan.IsEnabled = Console.ReadLine()?.ToLower() == "y";

            if (_settings.AutomatedScan.IsEnabled)
            {
                Console.Write("Enter hour for scan (0-23): ");
                if (int.TryParse(Console.ReadLine(), out int hour) && hour >= 0 && hour <= 23)
                {
                    _settings.AutomatedScan.ScheduledHour = hour;
                }

                Console.Write("Enter minute for scan (0-59): ");
                if (int.TryParse(Console.ReadLine(), out int minute) && minute >= 0 && minute <= 59)
                {
                    _settings.AutomatedScan.ScheduledMinute = minute;
                }

                Console.WriteLine("\nSelect scans to run:");

                Console.Write("Run ARP scan? (y/n): ");
                _settings.AutomatedScan.RunARPScan = Console.ReadLine()?.ToLower() == "y";

                Console.Write("Run ICMP scan? (y/n): ");
                _settings.AutomatedScan.RunICMPScan = Console.ReadLine()?.ToLower() == "y";

                Console.Write("Run Fragmented ICMP scan? (y/n): ");
                _settings.AutomatedScan.RunFragmentedICMPScan = Console.ReadLine()?.ToLower() == "y";

                Console.Write("Run Port scan? (y/n): ");
                _settings.AutomatedScan.RunPortScan = Console.ReadLine()?.ToLower() == "y";

                if (_selectedInterface != null)
                {
                    if (_automatedScanner == null || !wasEnabled)
                    {
                        _automatedScanner = new AutomatedScanner(_settings, _logger, _selectedInterface);
                        _logger.LogInformation("Automated scanner initialized");
                    }
                }
            }
            else
            {
                _automatedScanner?.Dispose();
                _automatedScanner = null;
            }

            await _settings.SaveAsync();

            // Show summary
            Console.WriteLine("\nAutomated Scan Configuration:");
            Console.WriteLine($"Status: {(_settings.AutomatedScan.IsEnabled ? "Enabled" : "Disabled")}");
            if (_settings.AutomatedScan.IsEnabled)
            {
                Console.WriteLine($"Scheduled time: {_settings.AutomatedScan.ScheduledHour:D2}:{_settings.AutomatedScan.ScheduledMinute:D2}");
                Console.WriteLine("Selected scans:");
                Console.WriteLine($"- ARP Scan: {(_settings.AutomatedScan.RunARPScan ? "Yes" : "No")}");
                Console.WriteLine($"- ICMP Scan: {(_settings.AutomatedScan.RunICMPScan ? "Yes" : "No")}");
                Console.WriteLine($"- Fragmented ICMP Scan: {(_settings.AutomatedScan.RunFragmentedICMPScan ? "Yes" : "No")}");
                Console.WriteLine($"- Port Scan: {(_settings.AutomatedScan.RunPortScan ? "Yes" : "No")}");
            }
        }

        private static async Task<bool> ConfirmAction(string message)
        {
            Console.Write($"\n{message} (y/n): ");
            string? response = Console.ReadLine()?.ToLower();
            return response == "y" || response == "yes";
        }
    }
}