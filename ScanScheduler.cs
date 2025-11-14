//using System.Net.NetworkInformation;
//using Microsoft.Extensions.Logging;

//namespace gradproject
//{
//    public class ScanScheduler : IDisposable
//    {
//        private readonly ScannerSettings _settings;
//        private readonly ILogger _logger;
//        private readonly Timer _scheduleCheckTimer;
//        private readonly AutomatedScanner _scanner;
//        private DateTime _nextScanTime;

//        public ScanScheduler(ScannerSettings settings, ILogger logger, NetworkInterface networkInterface)
//        {
//            _settings = settings;
//            _logger = logger;
//            _scanner = new AutomatedScanner(settings, logger, networkInterface);
//            _scheduleCheckTimer = new Timer(CheckSchedule, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
//            CalculateNextScanTime();
//        }

//        private void CalculateNextScanTime()
//        {
//            var now = DateTime.Now;
//            _nextScanTime = new DateTime(
//                now.Year, now.Month, now.Day,
//                _settings.AutomatedScan.ScheduledHour,
//                _settings.AutomatedScan.ScheduledMinute,
//                0);

//            if (_nextScanTime <= now)
//            {
//                _nextScanTime = _nextScanTime.AddDays(1);
//            }

//            _logger.LogInformation("Next scan scheduled for: {NextScanTime}", _nextScanTime);
//            Console.WriteLine($"Next automated scan scheduled for: {_nextScanTime:yyyy-MM-dd HH:mm}");
//        }

//        private async void CheckSchedule(object? state)
//        {
//            if (!_settings.AutomatedScan.IsEnabled)
//                return;

//            var now = DateTime.Now;
//            if (now >= _nextScanTime)
//            {
//                try
//                {
//                    await _scanner.RunScan();
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error during scheduled scan");
//                }
//                finally
//                {
//                    CalculateNextScanTime();
//                }
//            }
//        }

//        public TimeSpan GetTimeUntilNextScan()
//        {
//            return _nextScanTime - DateTime.Now;
//        }

//        public void Start()
//        {
//            if (_settings.AutomatedScan.IsEnabled)
//            {
//                _scheduleCheckTimer.Change(0, 60000); // Check every minute
//                _logger.LogInformation("Scan scheduler started");
//                Console.WriteLine("Automated scan scheduler started");
//                Console.WriteLine($"Next scan scheduled for: {_nextScanTime:yyyy-MM-dd HH:mm}");
//            }
//        }

//        public void Stop()
//        {
//            _scheduleCheckTimer.Change(Timeout.Infinite, Timeout.Infinite);
//            _logger.LogInformation("Scan scheduler stopped");
//            Console.WriteLine("Automated scan scheduler stopped");
//        }

//        public void UpdateSchedule()
//        {
//            CalculateNextScanTime();
//            if (_settings.AutomatedScan.IsEnabled)
//            {
//                Start();
//            }
//            else
//            {
//                Stop();
//            }
//        }

//        public void Dispose()
//        {
//            _scheduleCheckTimer?.Dispose();
//            _scanner?.Dispose();
//        }
//    }
//}