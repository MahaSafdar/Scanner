using System.Text.Json;
using Microsoft.Extensions.Logging;
using gradproject.models;

namespace gradproject
{
    public class NVDService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private const string NVD_API_URL = "https://services.nvd.nist.gov/rest/json/cves/2.0";
        private readonly string _apiKey;

        public NVDService(ILogger logger, string? apiKey = null)
        {
            _logger = logger;
            _apiKey = apiKey ?? string.Empty;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GradProject-VulnerabilityScanner/1.0");

            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("apiKey", _apiKey);
                _logger.LogInformation("NVD API key configured");
            }
            else
            {
                _logger.LogWarning("No NVD API key provided. Rate limits will be restricted.");
            }
        }

        public async Task<List<CVEEntry>> SearchVulnerabilities(string cpeName, DateTime? startDate = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"cpeName={Uri.EscapeDataString(cpeName)}"
                };

                if (startDate.HasValue)
                {
                    queryParams.Add($"pubStartDate={startDate.Value:yyyy-MM-ddTHH:mm:ss.fff}");
                }

                // Add result size limit and sort by severity
                queryParams.Add("resultsPerPage=100");
                queryParams.Add("sortBy=cvssV3Severity");
                queryParams.Add("sortOrder=desc");

                var url = $"{NVD_API_URL}?{string.Join("&", queryParams)}";
                _logger.LogDebug("Querying NVD API: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                // Handle rate limiting
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("NVD API rate limit reached. Waiting before retry...");
                    await Task.Delay(2000); // Wait 2 seconds before retry
                    response = await _httpClient.GetAsync(url);
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var nvdResponse = JsonSerializer.Deserialize<NVDResponse>(content);

                if (nvdResponse?.Vulnerabilities == null || !nvdResponse.Vulnerabilities.Any())
                {
                    _logger.LogInformation("No vulnerabilities found for {CpeName}", cpeName);
                    return new List<CVEEntry>();
                }

                var entries = ConvertToCVEEntries(nvdResponse.Vulnerabilities);
                _logger.LogInformation("Found {Count} vulnerabilities for {CpeName}",
                    entries.Count, cpeName);

                return entries;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error querying NVD API for {CpeName}. Status: {Status}",
                    cpeName, ex.StatusCode);
                return new List<CVEEntry>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying NVD API for {CpeName}", cpeName);
                return new List<CVEEntry>();
            }
        }

        private List<CVEEntry> ConvertToCVEEntries(List<NVDVulnerability> vulnerabilities)
        {
            var entries = new List<CVEEntry>();

            foreach (var vuln in vulnerabilities)
            {
                if (vuln.Cve == null) continue;

                try
                {
                    var entry = new CVEEntry
                    {
                        CVE = vuln.Cve.Id ?? "",
                        Name = vuln.Cve.Descriptions?.FirstOrDefault(d => d.Lang == "en")?.Value ?? "",
                        Description = string.Join(" ", vuln.Cve.Descriptions?
                            .Where(d => d.Lang == "en")
                            .Select(d => d.Value ?? "") ?? Array.Empty<string>()),
                        Severity = DetermineSeverity(vuln.Cve.Metrics),
                        AffectedVersions = ExtractAffectedVersions(vuln.Cve.Configurations),
                        References = ExtractReferences(vuln.Cve.References),
                        Recommendation = GenerateRecommendation(vuln.Cve)
                    };

                    entries.Add(entry);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error converting vulnerability entry for {CVE}",
                        vuln.Cve.Id);
                }
            }

            return entries;
        }

        private VulnerabilitySeverity DetermineSeverity(NVDMetrics? metrics)
        {
            if (metrics == null) return VulnerabilitySeverity.Unknown;

            // Try CVSS v3.1 first
            var score = metrics.CvssMetricV31?.FirstOrDefault()?.CvssData?.BaseScore
                ?? metrics.CvssMetricV3?.FirstOrDefault()?.CvssData?.BaseScore
                ?? metrics.CvssMetricV2?.FirstOrDefault()?.CvssData?.BaseScore
                ?? 0;

            return score switch
            {
                >= 9.0m => VulnerabilitySeverity.Critical,
                >= 7.0m => VulnerabilitySeverity.High,
                >= 4.0m => VulnerabilitySeverity.Medium,
                > 0.0m => VulnerabilitySeverity.Low,
                _ => VulnerabilitySeverity.Unknown
            };
        }

        private string[] ExtractAffectedVersions(List<NVDConfiguration>? configurations)
        {
            var versions = new HashSet<string>();

            if (configurations == null) return Array.Empty<string>();

            foreach (var config in configurations)
            {
                foreach (var node in config.Nodes ?? Enumerable.Empty<NVDNode>())
                {
                    foreach (var cpeMatch in node.CpeMatch ?? Enumerable.Empty<NVDCpeMatch>())
                    {
                        if (cpeMatch.Vulnerable)
                        {
                            if (!string.IsNullOrEmpty(cpeMatch.VersionStartIncluding))
                                versions.Add($">={cpeMatch.VersionStartIncluding}");
                            if (!string.IsNullOrEmpty(cpeMatch.VersionEndIncluding))
                                versions.Add($"<={cpeMatch.VersionEndIncluding}");
                            if (!string.IsNullOrEmpty(cpeMatch.VersionStartExcluding))
                                versions.Add($">{cpeMatch.VersionStartExcluding}");
                            if (!string.IsNullOrEmpty(cpeMatch.VersionEndExcluding))
                                versions.Add($"<{cpeMatch.VersionEndExcluding}");
                        }
                    }
                }
            }

            return versions.ToArray();
        }

        private List<string> ExtractReferences(List<NVDReference>? references)
        {
            return references?
                .Where(r => !string.IsNullOrEmpty(r.Url))
                .Select(r => r.Url!)
                .ToList() ?? new List<string>();
        }

        private string GenerateRecommendation(NVDCve cve)
        {
            var patchRefs = cve.References?
                .Where(r => r.Tags?.Any(t =>
                    t.Contains("Patch", StringComparison.OrdinalIgnoreCase) ||
                    t.Contains("Solution", StringComparison.OrdinalIgnoreCase)) ?? false)
                .Select(r => r.Url)
                .Where(url => !string.IsNullOrEmpty(url))
                .ToList();

            if (patchRefs?.Any() == true)
            {
                return $"Available fixes: {string.Join(", ", patchRefs)}";
            }

            return "Update to the latest version and follow vendor security advisories.";
        }
    }
}