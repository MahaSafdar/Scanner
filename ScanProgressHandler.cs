using System.Text;

namespace gradproject
{
    public class ScanProgressHandler : IProgress<string>
    {
        private int _totalPorts;
        private int _scannedPorts;
        private int _lastPercentage = -1;
        private readonly object _lock = new();

        public ScanProgressHandler(int totalPorts)
        {
            _totalPorts = totalPorts;
            _scannedPorts = 0;
        }

        public void Report(string value)
        {
            lock (_lock)
            {
                if (value.StartsWith("Scanning port"))
                {
                    _scannedPorts++;
                    UpdateProgressBar();
                }
                else
                {
                    // For non-progress messages, print on new line
                    Console.WriteLine($"\n{value}");
                }
            }
        }

        private void UpdateProgressBar()
        {
            int percentage = (_scannedPorts * 100) / _totalPorts;
            if (percentage == _lastPercentage) return;
            _lastPercentage = percentage;

            Console.CursorLeft = 0;
            Console.Write($"Progress: [");

            int progressBlocks = percentage / 2; // Each block represents 2%
            for (int i = 0; i < 50; i++)
            {
                if (i < progressBlocks)
                    Console.Write("█");
                else
                    Console.Write("░");
            }

            Console.Write($"] {percentage}% ({_scannedPorts}/{_totalPorts} ports)");
        }
    }
}