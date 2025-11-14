using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using gradproject.models;

namespace gradproject
{
    public class ScanReport
    {
        public DateTime ScanTime { get; set; }
        public string ScanType { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public List<ScanResult> Results { get; set; } = new();
        public List<PortScanResult> PortResults { get; set; } = new();
        public List<VulnerabilityResult> VulnerabilityResults { get; set; } = new();
        public TimeSpan TotalDuration { get; set; }
        public string Summary { get; set; } = string.Empty;

        public async Task SaveAsHtmlAsync(string filename)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename)!);
            await File.WriteAllTextAsync(filename, GenerateHtmlReport());
        }

        public async Task SaveAsJsonAsync(string filename)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename)!);
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(filename, json);
        }

        private string GenerateHtmlReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head>");
            sb.AppendLine("<style>");
            sb.AppendLine(@"
                body { font-family: Arial, sans-serif; margin: 20px; }
                .header { background-color: #f8f9fa; padding: 20px; border-radius: 5px; }
                .results { margin-top: 20px; }
                table { width: 100%; border-collapse: collapse; }
                th, td { padding: 8px; text-align: left; border: 1px solid #ddd; }
                th { background-color: #f2f2f2; }
                .severity-high { background-color: #ffebee; }
                .severity-medium { background-color: #fff3e0; }
                .severity-low { background-color: #f1f8e9; }
            ");
            sb.AppendLine("</style></head><body>");

            // Header Section
            sb.AppendLine("<div class='header'>");
            sb.AppendLine($"<h1>Scan Report</h1>");
            sb.AppendLine($"<p>Scan Type: {ScanType}</p>");
            sb.AppendLine($"<p>Target: {Target}</p>");
            sb.AppendLine($"<p>Scan Time: {ScanTime}</p>");
            sb.AppendLine($"<p>Duration: {TotalDuration.TotalSeconds:F2} seconds</p>");
            sb.AppendLine("</div>");

            // Port Scan Results
            if (PortResults.Any())
            {
                sb.AppendLine("<h2>Port Scan Results</h2>");
                sb.AppendLine("<div class='results'>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Port</th><th>Status</th><th>Service</th><th>Version</th><th>OS</th><th>Response Time</th></tr>");

                foreach (var result in PortResults.OrderBy(p => p.Port))
                {
                    string severityClass = result.IsOpen ? "severity-high" : "severity-low";
                    sb.AppendLine($"<tr class='{severityClass}'>");
                    sb.AppendLine($"<td>{result.Port}</td>");
                    sb.AppendLine($"<td>{(result.IsOpen ? "Open" : "Closed")}</td>");
                    sb.AppendLine($"<td>{result.ServiceName}</td>");
                    sb.AppendLine($"<td>{result.ServiceVersion}</td>");
                    sb.AppendLine($"<td>{result.OperatingSystem}</td>");
                    sb.AppendLine($"<td>{result.ScanDuration.TotalMilliseconds:F2}ms</td>");
                    sb.AppendLine("</tr>");
                }

                sb.AppendLine("</table></div>");
            }

            // Vulnerability Results
            if (VulnerabilityResults.Any())
            {
                sb.AppendLine("<h2>Vulnerability Results</h2>");
                sb.AppendLine("<div class='results'>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Severity</th><th>Name</th><th>CVE</th><th>Service</th><th>Version</th></tr>");

                foreach (var result in VulnerabilityResults.OrderByDescending(v => v.Severity))
                {
                    string severityClass = $"severity-{result.Severity.ToString().ToLower()}";
                    sb.AppendLine($"<tr class='{severityClass}'>");
                    sb.AppendLine($"<td>{result.Severity}</td>");
                    sb.AppendLine($"<td>{result.Name}</td>");
                    sb.AppendLine($"<td>{result.CVE}</td>");
                    sb.AppendLine($"<td>{result.AffectedService}</td>");
                    sb.AppendLine($"<td>{result.AffectedVersion}</td>");
                    sb.AppendLine("</tr>");
                }

                sb.AppendLine("</table></div>");
            }

            // Generic Results
            if (Results.Any())
            {
                sb.AppendLine("<h2>Other Results</h2>");
                sb.AppendLine("<div class='results'>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Identifier</th><th>Status</th><th>Service</th><th>Version</th><th>Response Time</th></tr>");

                foreach (var result in Results)
                {
                    string severityClass = result.GetSeverityClass();
                    sb.AppendLine($"<tr class='{severityClass}'>");
                    sb.AppendLine($"<td>{result.Identifier}</td>");
                    sb.AppendLine($"<td>{result.Status}</td>");
                    sb.AppendLine($"<td>{result.ServiceName}</td>");
                    sb.AppendLine($"<td>{result.Version}</td>");
                    sb.AppendLine($"<td>{result.ResponseTime.TotalMilliseconds:F2}ms</td>");
                    sb.AppendLine("</tr>");
                }

                sb.AppendLine("</table></div>");
            }

            if (!Results.Any() && !PortResults.Any() && !VulnerabilityResults.Any())
            {
                sb.AppendLine("<p>No results found.</p>");
            }

            sb.AppendLine("</body></html>");
            return sb.ToString();
        }
    }

    public class ScanResult
    {
        public string Identifier { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public TimeSpan ResponseTime { get; set; }

        public string GetSeverityClass()
        {
            return Status.ToLower() switch
            {
                "open" => "severity-high",
                "filtered" => "severity-medium",
                _ => "severity-low"
            };
        }
    }
}