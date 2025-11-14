using System.Collections.Concurrent;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using gradproject.models;

namespace gradproject
{
    public class PortScannerc : IDisposable
    {
        private readonly ScannerConfiguration _config;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _throttler;
        private bool _disposed;
        private DateTime _lastProgressUpdate;

        private class ScanMetrics
        {
            public int ScannedPorts;
            public int SuccessfulConnections;
            public int FailedConnections;
        }

        private readonly ScanMetrics _metrics = new();

        public PortScannerc(ScannerConfiguration config, ILogger logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _throttler = new SemaphoreSlim(config.MaxConcurrentScans);
        }


        public async Task<List<PortScanResult>> ScanAsync(
    string target,
    IEnumerable<int> ports,
    ServiceProber prober,
    IProgress<string>? progresses = null,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
            if (ports == null) throw new ArgumentNullException(nameof(ports));
            if (prober == null) throw new ArgumentNullException(nameof(prober));

            var results = new ConcurrentBag<PortScanResult>();
            var portList = ports.ToList();
            var totalPorts = portList.Count;
            var startTime = DateTime.Now;

            try
            {
                _logger.LogInformation("Starting port scan on {Target} for {PortCount} ports", target, totalPorts);
                await ScanPortsInParallel(target, portList, prober, results, progresses, cancellationToken);

                var duration = DateTime.Now - startTime;
                LogScanMetrics(duration);
                return results.OrderBy(r => r.Port).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during port scan");
                throw;
            }
        }
        /* public async Task<List<PortScanResult>> ScanAsync(
     string target,
     IEnumerable<int> ports,
     ServiceProber prober,
     IProgress<string>? progresses = null,
     CancellationToken cancellationToken = default)
         {
             if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
             if (ports == null) throw new ArgumentNullException(nameof(ports));
             if (prober == null) throw new ArgumentNullException(nameof(prober));

             var results = new ConcurrentBag<PortScanResult>();
             var portList = ports.ToList();
             var totalPorts = portList.Count;
             var startTime = DateTime.Now;
             var processedPorts = 0;

             try
             {
                 _logger.LogInformation("Starting port scan on {Target} for {PortCount} ports", target, totalPorts);

                 // Break ports into smaller chunks to handle large ranges
                 int chunkSize = 1000;
                 for (int i = 0; i < portList.Count; i += chunkSize)
                 {
                     var chunk = portList.Skip(i).Take(chunkSize).ToList();
                     await ScanPortsInParallel(target, chunk, prober, results, progresses, cancellationToken);

                     processedPorts += chunk.Count;
                     var percentage = (processedPorts * 100) / totalPorts;
                     progresses?.Report($"Progress: {percentage}% | Open ports: {results.Count(r => r.IsOpen)}");

                     // Small delay between chunks to prevent overwhelming the target
                     await Task.Delay(100, cancellationToken);
                 }

                 var duration = DateTime.Now - startTime;
                 LogScanMetrics(duration);
                 return results.OrderBy(r => r.Port).ToList();
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Error during port scan");
                 throw;
             }
         }*/

        private async Task ScanPortsInParallel(
            string target,
            List<int> ports,
            ServiceProber prober,
            ConcurrentBag<PortScanResult> results,
            IProgress<string>? progresses,
            CancellationToken cancellationToken)
        {
            int batchSize = Math.Max(50, Math.Min(200, Environment.ProcessorCount * 25)); //25 batches for each processor core
            var batches = ports.Chunk(batchSize).ToList();
            var totalBatches = batches.Count;
            var completedBatches = 0;
            var progressThrottle = new SemaphoreSlim(1);

            await Parallel.ForEachAsync(batches,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = _config.MaxConcurrentScans,
                    CancellationToken = cancellationToken
                },
                async (batch, ct) =>
                {
                    var batchTasks = batch.Select(port =>
                        ScanPortAsync(target, port, prober, results, ct));

                    await Task.WhenAll(batchTasks);

                    var batchCompleted = Interlocked.Increment(ref completedBatches);
                    await UpdateProgressAsync(batchCompleted, totalBatches, results.Count(r => r.IsOpen), progresses, progressThrottle);
                });
        }

        

        

         private async Task UpdateProgressAsync(
             int completedBatches,
             int totalBatches,
             int openPorts,
             IProgress<string>? progresses,
             SemaphoreSlim progressThrottle)
         {
             if (progresses != null && await progressThrottle.WaitAsync(0))
             {
                 try
                 {
                     var now = DateTime.Now;
                     if ((now - _lastProgressUpdate).TotalMilliseconds == 500)
                     {
                         var percentage = (completedBatches * 100) / totalBatches;
                         progresses.Report($"Progress: {percentage}% complete | Open ports found: {openPorts}");
                         _lastProgressUpdate = now;
                        await Task.Delay(10);
                    }
                 }
                 finally
                 {
                     progressThrottle.Release();
                 }
                await Task.Delay(10);
             }
         }
        /*private async Task ScanPortAsync(
                    string target,
                    int port,
                    ServiceProber prober,
                    ConcurrentBag<PortScanResult> results,
                    CancellationToken cancellationToken)
        {
            await _throttler.WaitAsync(cancellationToken);
            var startTime = DateTime.Now;

            try
            {
                using var client = new TcpClient();
                using var timeoutCts = new CancellationTokenSource(_config.ConnectionTimeout);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    timeoutCts.Token, cancellationToken);

                await client.ConnectAsync(target, port, linkedCts.Token);

                if (client.Connected)
                {
                    Interlocked.Increment(ref _metrics.SuccessfulConnections);
                    var scanDuration = DateTime.Now - startTime;

                    // Get service information
                    var serviceInfo = await prober.ProbeServiceAsync(target, port, linkedCts.Token);

                    results.Add(new PortScanResult
                    {
                        Port = port,
                        IsOpen = true,
                        ServiceName = serviceInfo.ServiceName,
                        ServiceVersion = serviceInfo.Version,
                        ServiceBanner = serviceInfo.Banner,
                        OperatingSystem = DetectOperatingSystem(serviceInfo.Banner, client.Client.Ttl),
                        ScanDuration = scanDuration,
                        ServiceDetails = new Dictionary<string, string>
                        {
                            { "Banner", serviceInfo.Banner },
                            { "Extra Info", serviceInfo.ExtraInfo },
                            { "Confidence", $"{serviceInfo.Confidence:P0}" },
                            { "Connection Time", $"{scanDuration.TotalMilliseconds:F2}ms" }
                        }
                    });
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception)
            {
                Interlocked.Increment(ref _metrics.FailedConnections);
            }
            finally
            {
                _throttler.Release();
                Interlocked.Increment(ref _metrics.ScannedPorts);
            }
        }*/

        public async Task ScanPortAsync(
    string target,
    int port,
    ServiceProber prober,
    ConcurrentBag<PortScanResult> results,
    CancellationToken cancellationToken)
        {
            await _throttler.WaitAsync(cancellationToken);
            var startTime = DateTime.Now;

            try
            {
                using var client = new TcpClient();
                client.ReceiveTimeout = _config.ConnectionTimeout;
                client.SendTimeout = _config.ConnectionTimeout;

                try
                {
                    // Simple connection attempt without cancellation token
                    await client.ConnectAsync(target, port);

                    if (client.Connected)
                    {
                        Interlocked.Increment(ref _metrics.SuccessfulConnections);
                        var scanDuration = DateTime.Now - startTime;

                        var serviceInfo = await prober.ProbeServiceAsync(target, port, cancellationToken);

                        results.Add(new PortScanResult
                        {
                            Port = port,
                            IsOpen = true,
                            ServiceName = serviceInfo.ServiceName,
                            ServiceVersion = serviceInfo.Version,
                            ServiceBanner = serviceInfo.Banner,
                            OperatingSystem = DetectOperatingSystem(serviceInfo.Banner, client.Client.Ttl),
                            ScanDuration = scanDuration,
                            ServiceDetails = new Dictionary<string, string>
                    {
                        { "Banner", serviceInfo.Banner },
                        { "Extra Info", serviceInfo.ExtraInfo },
                        { "Confidence", $"{serviceInfo.Confidence:P0}" },
                        { "Connection Time", $"{scanDuration.TotalMilliseconds:F2}ms" }
                    }
                        });
                    }
                }
                catch (Exception)
                {
                    // Connection failed - treat as closed port
                    Interlocked.Increment(ref _metrics.FailedConnections);
                }
            }
            catch (Exception)
            {
                Interlocked.Increment(ref _metrics.FailedConnections);
            }
            finally
            {
                _throttler.Release();
                Interlocked.Increment(ref _metrics.ScannedPorts);
            }
        }

        private string DetectOperatingSystem(string banner, short ttl)
        {
            // First try TTL-based detection
            var ttlBasedOS = ttl switch
            {
                <= 64 => "Linux/Unix (TTL: 64)",
                <= 128 => "Windows (TTL: 128)",
                <= 255 => "Network Device (TTL: 255)",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(ttlBasedOS))
                return ttlBasedOS;

            // Try banner-based detection
            if (string.IsNullOrEmpty(banner))
                return string.Empty;

            var lowerBanner = banner.ToLower();

            if (lowerBanner.Contains("microsoft") ||
                lowerBanner.Contains("windows") ||
                lowerBanner.Contains("win32") ||
                lowerBanner.Contains("win64"))
                return "Windows (from banner)";

            if (lowerBanner.Contains("linux") ||
                lowerBanner.Contains("ubuntu") ||
                lowerBanner.Contains("debian") ||
                lowerBanner.Contains("centos") ||
                lowerBanner.Contains("fedora") ||
                lowerBanner.Contains("redhat") ||
                lowerBanner.Contains("unix"))
                return "Linux (from banner)";

            if (lowerBanner.Contains("macos") ||
                lowerBanner.Contains("darwin") ||
                lowerBanner.Contains("mac os") ||
                lowerBanner.Contains("apple"))
                return "MacOS (from banner)";

            return string.Empty;
        }

        private void LogScanMetrics(TimeSpan duration)
        {
            _logger.LogInformation(
                "Scan completed in {Duration:F2} seconds. " +
                "Ports scanned: {ScannedPorts}, " +
                "Open ports: {OpenPorts}, " +
                "Failed attempts: {FailedConnections}",
                duration.TotalSeconds,
                _metrics.ScannedPorts,
                _metrics.SuccessfulConnections,
                _metrics.FailedConnections);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _throttler?.Dispose();
                _disposed = true;
            }
        }
    }
}