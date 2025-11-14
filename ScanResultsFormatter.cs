using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using gradproject.models;
namespace gradproject
{
    public static class ScanResultsFormatter
    {
        private static class ConsoleColors
        {
            public const string Reset = "\u001b[0m";
            public const string Red = "\u001b[31m";
            public const string Green = "\u001b[32m";
            public const string Yellow = "\u001b[33m";
            public const string Blue = "\u001b[34m";
            public const string Magenta = "\u001b[35m";
            public const string Cyan = "\u001b[36m";
        }

        public static string FormatPortScanResults(IEnumerable<PortScanResult> results)
        {
            var sb = new StringBuilder();
            var openPorts = results.Where(r => r.IsOpen).ToList();

            // Header with time
            sb.AppendLine($"=== Port Scan Results ===");
            sb.AppendLine($"Scan Time: {DateTime.Now}");
            sb.AppendLine($"Duration: {results.First().ScanDuration.TotalSeconds:F2} seconds");
            sb.AppendLine("=========================\n");

            // Pretty border
            sb.AppendLine("╔══════════════════════════════════════════════════╗");
            sb.AppendLine("║                PORT SCAN RESULTS                 ║");
            sb.AppendLine("╚══════════════════════════════════════════════════╝");

            // Summary
            sb.AppendLine($"Total Ports Scanned: {results.Count()}");
            sb.AppendLine($"Open Ports: {openPorts.Count}");
            sb.AppendLine($"Closed Ports: {results.Count() - openPorts.Count}");
            sb.AppendLine("──────────────────────────────────────────────────\n");

            if (!openPorts.Any())
            {
                sb.AppendLine("\nNo open ports were found.");
                return sb.ToString();
            }

            // List each open port individually
            foreach (var port in openPorts.OrderBy(p => p.Port))
            {
                sb.AppendLine($"► Port: {port.Port}");
                sb.AppendLine($"  Service: {port.ServiceName}");

                if (!string.IsNullOrEmpty(port.ServiceVersion))
                    sb.AppendLine($"  ├─ Version: {port.ServiceVersion}");

                if (!string.IsNullOrEmpty(port.OperatingSystem))
                    sb.AppendLine($"  ├─ OS Detection: {port.OperatingSystem}");

                sb.AppendLine($"  └─ Response Time: {port.ScanDuration.TotalMilliseconds:F2}ms");
                sb.AppendLine(); // Empty line between ports
            }

            return sb.ToString();
        }

        public static string FormatVulnerabilityResults(IEnumerable<VulnerabilityResult> results)
        {
            var sb = new StringBuilder();
            var vulnerabilities = results.OrderByDescending(v => v.Severity).ToList();

            // Header
            sb.AppendLine("╔══════════════════════════════════════════════════╗");
            sb.AppendLine("║            VULNERABILITY SCAN RESULTS            ║");
            sb.AppendLine("╚══════════════════════════════════════════════════╝");

            // Summary
            var severityCounts = new Dictionary<VulnerabilitySeverity, int>
            {
                { VulnerabilitySeverity.Critical, vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Critical) },
                { VulnerabilitySeverity.High, vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.High) },
                { VulnerabilitySeverity.Medium, vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Medium) },
                { VulnerabilitySeverity.Low, vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Low) }
            };

            sb.AppendLine("Summary:");
            foreach (var severity in severityCounts.Where(s => s.Value > 0))
            {
                var color = GetSeverityColor(severity.Key);
                sb.AppendLine($"{color}  {severity.Key}: {severity.Value}{ConsoleColors.Reset}");
            }

            if (!vulnerabilities.Any())
            {
                sb.AppendLine("\nNo vulnerabilities were detected.");
                return sb.ToString();
            }

            sb.AppendLine("\nDetailed Findings:");
            sb.AppendLine("──────────────────────────────────────────────────");

            foreach (var vuln in vulnerabilities)
            {
                var color = GetSeverityColor(vuln.Severity);

                sb.AppendLine($"\n{color}[{vuln.Severity}] {vuln.Name}{ConsoleColors.Reset}");
                sb.AppendLine($"CVE ID: {vuln.CVE}");
                sb.AppendLine($"Affected Service: {vuln.AffectedService} {vuln.AffectedVersion}");

                // Description - word wrap for better readability
                sb.AppendLine("Description:");
                foreach (var line in WordWrap(vuln.Description, 60))
                {
                    sb.AppendLine($"  {line}");
                }

                // Recommendation
                sb.AppendLine("Recommendation:");
                foreach (var line in WordWrap(vuln.Recommendation, 60))
                {
                    sb.AppendLine($"  {line}");
                }

                // References (limited to top 3 for clarity)
                if (vuln.References?.Any() == true)
                {
                    sb.AppendLine("References:");
                    foreach (var reference in vuln.References.Take(3))
                    {
                        sb.AppendLine($"  └─ {reference}");
                    }
                }

                sb.AppendLine("──────────────────────────────────────────────────");
            }

            return sb.ToString();
        }

        private static string GetSeverityColor(VulnerabilitySeverity severity) => severity switch
        {
            VulnerabilitySeverity.Critical => ConsoleColors.Red,
            VulnerabilitySeverity.High => ConsoleColors.Magenta,
            VulnerabilitySeverity.Medium => ConsoleColors.Yellow,
            VulnerabilitySeverity.Low => ConsoleColors.Blue,
            _ => ConsoleColors.Reset
        };

        private static IEnumerable<string> WordWrap(string text, int width)
        {
            if (string.IsNullOrEmpty(text)) yield break;

            var words = text.Split(' ');
            var line = new StringBuilder();

            foreach (var word in words)
            {
                if (line.Length + word.Length + 1 > width)
                {
                    if (line.Length > 0)
                    {
                        yield return line.ToString();
                        line.Clear();
                    }
                    // If the word itself is too long, split it
                    if (word.Length > width)
                    {
                        var parts = SplitLongWord(word, width);
                        foreach (var part in parts.Take(parts.Count - 1))
                        {
                            yield return part;
                        }
                        line.Append(parts.Last());
                    }
                    else
                    {
                        line.Append(word);
                    }
                }
                else
                {
                    if (line.Length > 0)
                    {
                        line.Append(' ');
                    }
                    line.Append(word);
                }
            }

            if (line.Length > 0)
            {
                yield return line.ToString();
            }
        }

        private static List<string> SplitLongWord(string word, int width)
        {
            var parts = new List<string>();
            for (int i = 0; i < word.Length; i += width)
            {
                parts.Add(word.Substring(i, Math.Min(width, word.Length - i)));
            }
            return parts;
        }
    }
}