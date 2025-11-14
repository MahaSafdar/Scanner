    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;

    namespace gradproject
    {
       public static class ICMP
        {
            public static async Task<string> PerformICMPScan(IPAddress startIP, IPAddress endIP, IProgress<string>? progress = null)
            {
                Console.WriteLine("\nPerforming ICMP scan...");
                return await ScanNetworkAsync(startIP, endIP, progress);
            }

            private static async Task<string> ScanNetworkAsync(IPAddress startIP, IPAddress endIP, IProgress<string>? progress = null)
            {
              var tasks = new List<Task<string>>();
              uint startNum = Utils.IpToUint(startIP);
              uint endNum = Utils.IpToUint(endIP);
              uint totalIPs = endNum - startNum + 1;
              uint completedIPs = 0;

              for (uint i = startNum; i <= endNum; i++)
              {
                IPAddress targetIP = Utils.UintToIp(i);
                tasks.Add(ScanHostAsync(targetIP));

                completedIPs++;
                int percentComplete = (int)((completedIPs * 100) / totalIPs);
                progress?.Report($"Progress: {percentComplete}%");
                await Task.Delay(10);
                
              }

                  string[] results = await Task.WhenAll(tasks);
                    return string.Join(Environment.NewLine, results);
            }

            private static async Task<string> ScanHostAsync(IPAddress address)
            {
                StringBuilder result = new StringBuilder();
                using (Ping pingSender = new Ping())
                {
                    try
                    {
                        PingReply reply = await pingSender.SendPingAsync(address, 1000);
                        if (reply.Status == IPStatus.Success)
                        {
                            string hostName = await GetHostNameAsync(address);
                            string os = OsDetection(reply.Options?.Ttl ?? 0);
                            result.AppendLine($"ICMP: Host {address} is alive. Hostname: {hostName}");
                            result.AppendLine($"      Operating System (guess): {os}");
                            result.AppendLine($"      TTL: {reply.Options?.Ttl ?? 0}");
                            result.AppendLine();
                        }
                    }
                    catch
                    {
                        // Ignore ping failures
                    }
                }
                return result.ToString();
            }

            public static async Task<string> GetHostNameAsync(IPAddress address)
            {
                try
                {
                    IPHostEntry hostEntry = await Dns.GetHostEntryAsync(address);
                    return hostEntry.HostName;
                }
                catch (Exception)
                {
                    return "Unavailable";
                }
            }

            public static string OsDetection(int ttl)
            {
                if (ttl <= 64)
                {
                    return "Linux/Unix/macOS/Android (probability: high)";
                }
                else if (ttl <= 128)
                {
                    return "Windows (probability: high)";
                }
                else if (ttl <= 255)
                {
                    return "Cisco/Network Device (probability: medium)";
                }
                else
                {
                    return "Unknown OS";
                }
            }
        }
    }