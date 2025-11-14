using System.Text.Json;

namespace gradproject
{
    public class ScannerSettings
    {
        private const string DefaultSettingsFile = "scanner-settings.json";

        public NetworkSettings Network { get; set; } = new();
        public LoggingSettings Logging { get; set; } = new();
        public AutomatedScanSettings AutomatedScan { get; set; } = new();

        public ScannerSettings()
        {
            Network = new NetworkSettings();
            Logging = new LoggingSettings();
            AutomatedScan = new AutomatedScanSettings();
        }

        public static async Task<ScannerSettings> LoadAsync()
        {
            try
            {
                if (File.Exists(DefaultSettingsFile))
                {
                    string jsonString = await File.ReadAllTextAsync(DefaultSettingsFile);
                    var settings = JsonSerializer.Deserialize<ScannerSettings>(jsonString);
                    return settings ?? new ScannerSettings();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }
            return new ScannerSettings();
        }

        public async Task SaveAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(this, options);
                await File.WriteAllTextAsync(DefaultSettingsFile, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
                throw;
            }
        }
    }

    public class NetworkSettings
    {
        public int MaxConcurrentScans { get; set; } = 100;
        public int ConnectionTimeout { get; set; } = 1000;
        public int RetryAttempts { get; set; } = 2;
        public int RetryDelay { get; set; } = 500;
        public bool EnableFragmentation { get; set; } = false;
    }

    public class LoggingSettings
    {
        public bool EnableDebugLogging { get; set; } = false;
        public string LogDirectory { get; set; } = "Logs";
        public bool SaveReportsAsHtml { get; set; } = true;
        public bool SaveReportsAsJson { get; set; } = true;
    }

    public class AutomatedScanSettings
    {
        public bool IsEnabled { get; set; } = false;
        public int ScheduledHour { get; set; } = 0;
        public int ScheduledMinute { get; set; } = 0;
        public bool RunARPScan { get; set; } = false;
        public bool RunICMPScan { get; set; } = false;
        public bool RunFragmentedICMPScan { get; set; } = false;
        public bool RunPortScan { get; set; } = false;
        public List<int> PortsToScan { get; set; } = new List<int> { 80, 443, 22, 21, 3389 };
    }
}