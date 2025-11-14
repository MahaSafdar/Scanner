using System.Xml.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Text.Json;
using gradproject.models; 

public class CPEDictionary
{
    private readonly Dictionary<string, List<(string vendor, string product)>> _serviceMapping = new();
    private readonly ILogger _logger;

    public CPEDictionary(ILogger logger)
    {
        _logger = logger;
        LoadCPEDictionary();
    }

    private void LoadCPEDictionary()
    {
        try
        {
            string cpeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "official-cpe-dictionary.xml");
            if (!File.Exists(cpeFilePath))
            {
                _logger.LogWarning("CPE dictionary file not found. Using basic service mapping.");
                LoadBasicMapping();
                return;
            }

            var doc = XDocument.Load(cpeFilePath);
            XNamespace ns = "http://cpe.mitre.org/dictionary/2.0";

            var items = doc.Descendants(ns + "cpe-item")
                          .Select(item => item.Element(ns + "title")?.Value)
                          .Where(title => title != null);

            foreach (var item in items)
            {
                var parts = item.Split(':');
                if (parts.Length >= 5)
                {
                    string vendor = parts[3];
                    string product = parts[4];
                    string serviceKey = product.ToLower();

                    if (!_serviceMapping.ContainsKey(serviceKey))
                    {
                        _serviceMapping[serviceKey] = new List<(string, string)>();
                    }
                    _serviceMapping[serviceKey].Add((vendor, product));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading CPE dictionary. Using basic mapping.");
            LoadBasicMapping();
        }
    }

    private void LoadBasicMapping()
    {
        var basicMapping = new Dictionary<string, (string vendor, string product)>
        {
            { "ftp", ("vsftpd", "vsftpd") },
            { "ssh", ("openssh", "openssh") },
            { "http", ("apache", "http_server") },
            { "nginx", ("nginx", "nginx") },
            { "mysql", ("oracle", "mysql") },
            { "postgresql", ("postgresql", "postgresql") },
            { "telnet", ("linux", "telnetd") },
            { "smb", ("microsoft", "smb") },
            { "rdp", ("microsoft", "remote_desktop") }
        };

        foreach (var kvp in basicMapping)
        {
            _serviceMapping[kvp.Key] = new List<(string, string)> { kvp.Value };
        }
    }

    public (string vendor, string product) GetCPEMapping(string serviceName, string banner = "")
    {
        serviceName = serviceName.ToLower();

        // Try exact match first
        if (_serviceMapping.TryGetValue(serviceName, out var matches))
        {
            // If we have a banner, try to find the best match
            if (!string.IsNullOrEmpty(banner))
            {
                var bestMatch = matches.FirstOrDefault(m =>
                    banner.Contains(m.vendor, StringComparison.OrdinalIgnoreCase) ||
                    banner.Contains(m.product, StringComparison.OrdinalIgnoreCase));

                if (bestMatch != default)
                    return bestMatch;
            }

            // Return first match if no better match found
            return matches[0];
        }

        // Try partial matches
        var partialMatches = _serviceMapping
            .Where(kvp => serviceName.Contains(kvp.Key) || kvp.Key.Contains(serviceName))
            .SelectMany(kvp => kvp.Value)
            .ToList();

        if (partialMatches.Any())
        {
            if (!string.IsNullOrEmpty(banner))
            {
                var bestMatch = partialMatches.FirstOrDefault(m =>
                    banner.Contains(m.vendor, StringComparison.OrdinalIgnoreCase) ||
                    banner.Contains(m.product, StringComparison.OrdinalIgnoreCase));

                if (bestMatch != default)
                    return bestMatch;
            }

            return partialMatches[0];
        }

        // If no match found, return the service name as both vendor and product
        return (serviceName, serviceName);
    }
}