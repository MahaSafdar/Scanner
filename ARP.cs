using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace gradproject
{
   public static class ARP
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int destIp, int srcIp, byte[] pMacAddr, ref uint phyAddrLen);

        public static async Task<string> PerformARPScan(IPAddress startIP, IPAddress endIP, IProgress<string>? progress = null)

        {

            Console.WriteLine("\nPerforming ARP scan...");
            string result = await ScanNetworkAsync(startIP, endIP, progress);
            Console.WriteLine("ARP scan complete.");
            return result;
        }

        private static async Task<string> ScanNetworkAsync(IPAddress startIP, IPAddress endIP, IProgress<string>? progress = null)
        {
            var tasks = new List<Task<string>>();
            var arpTable = new Dictionary<IPAddress, PhysicalAddress>();

            uint startNum = Utils.IpToUint(startIP);
            uint endNum = Utils.IpToUint(endIP);
            uint totalIPs = endNum - startNum + 1;
            uint completedIPs = 0;

            for (uint i = startNum; i <= endNum; i++)
            {
                IPAddress currentIP = Utils.UintToIp(i);
                tasks.Add(SendARPRequestAsync(currentIP, arpTable));

                completedIPs++;
                int percentComplete = (int)((completedIPs * 100) / totalIPs);
                progress?.Report($"Progress: {percentComplete}%");

                // Add a small delay to see progress more clearly
                await Task.Delay(10);
            }

            await Task.WhenAll(tasks);
            return FormatARPTable(arpTable);
        }

        private static async Task<string> SendARPRequestAsync(IPAddress targetIP, Dictionary<IPAddress, PhysicalAddress> arpTable)
        {
            return await Task.Run(() =>
            {
                byte[] macAddr = new byte[6];
                uint macAddrLen = (uint)macAddr.Length;

                int result = SendARP(
                    BitConverter.ToInt32(targetIP.GetAddressBytes(), 0),
                    0,
                    macAddr,
                    ref macAddrLen
                );

                if (result == 0)
                {
                    PhysicalAddress physicalAddress = new PhysicalAddress(macAddr);
                    lock (arpTable)
                    {
                        if (!arpTable.ContainsKey(targetIP))
                        {
                            arpTable[targetIP] = physicalAddress;
                        }
                    }
                    return $"IP: {targetIP}, MAC: {physicalAddress}";
                }
                return string.Empty;
            });
        }

        private static string FormatARPTable(Dictionary<IPAddress, PhysicalAddress> arpTable)
        {
            var result = new StringBuilder();
            result.AppendLine("ARP Scan Results:");
            foreach (var entry in arpTable.OrderBy(e => Utils.IpToUint(e.Key)))
            {
                result.AppendLine($"IP: {entry.Key}, MAC: {entry.Value}");
            }
            return result.ToString();
        }
    }
}