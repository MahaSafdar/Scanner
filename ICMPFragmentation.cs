        using System.Net;
        using System.Net.Sockets;
        using System.Text;
        using System.Runtime.Versioning;
        using System.Security.Principal;

        namespace gradproject
        {
           public static class ICMPFragmentation
            {
                private const int TIMEOUT_MS = 1000;
                private const int MAX_CONCURRENT_SCANS = 100;
                private const int BUFFER_SIZE = 1024;
                private const int PACKET_SIZE = 576;

                public static async Task<string> PerformFragmentedICMPScan(IPAddress startIP, IPAddress endIP, IProgress<string>? progress = null)
                {
                    Console.WriteLine("\nPerforming fragmented ICMP scan...");
                    return await ScanNetworkWithFragmentationAsync(startIP, endIP, progress);
                }

                private static async Task<string> ScanNetworkWithFragmentationAsync(IPAddress startIP, IPAddress endIP, IProgress<string>? progress = null)
                {
                  using var semaphore = new SemaphoreSlim(MAX_CONCURRENT_SCANS);
                 var tasks = new List<Task<string>>();
                 uint totalHosts = Utils.IpToUint(endIP) - Utils.IpToUint(startIP) + 1;
                 uint scannedHosts = 0;
                 uint respondingHosts = 0;

                   for (uint i = Utils.IpToUint(startIP); i <= Utils.IpToUint(endIP); i++)
            {
                IPAddress targetIP = Utils.UintToIp(i);
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    var result = new StringBuilder();
                    try
                    {
                        bool responded = await ScanHostWithFragmentationAsync(targetIP, result);
                        if (responded) Interlocked.Increment(ref respondingHosts);
                    }
                    finally
                    {
                        semaphore.Release();
                        uint scanned = Interlocked.Increment(ref scannedHosts);
                        int percentComplete = (int)((scanned * 100) / totalHosts);
                        progress?.Report($"Progress: {percentComplete}%");
                    }
                    return result.ToString();
                }));
            }

            try
                    {
                        string[] results = await Task.WhenAll(tasks);
                        string scanResult = string.Join(Environment.NewLine,
                            results.Where(r => !string.IsNullOrWhiteSpace(r)));
                        Console.WriteLine($"Scan complete. {respondingHosts} out of {totalHosts} hosts responded.");
                        return scanResult;
                    }
                    catch (Exception)
                    {
                        return "Error completing the scan. Some results may be incomplete.";
                    }
                }

                private static async Task<bool> ScanHostWithFragmentationAsync(IPAddress address, StringBuilder result)
                {
                    // Ensure we have admin privileges for raw socket
                    if (!IsAdministrator())
                    {
                        result.AppendLine("Error: Administrator privileges required for fragmented ICMP scan.");
                        return false;
                    }

                    using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
                    try
                    {
                        ConfigureSocket(socket);
                        byte[] packet = CreateFragmentedIcmpPacket();

                        var endpoint = new IPEndPoint(address, 0);
                        await socket.SendToAsync(new ArraySegment<byte>(packet), SocketFlags.None, endpoint);

                        byte[] buffer = new byte[BUFFER_SIZE];
                        using var cts = new CancellationTokenSource(TIMEOUT_MS);

                        try
                        {
                            var receiveTask = socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                            if (await Task.WhenAny(receiveTask, Task.Delay(TIMEOUT_MS, cts.Token)) == receiveTask)
                            {
                                int bytes = await receiveTask;
                                if (bytes > 0)
                                {
                                    await ProcessSuccessfulResponse(address, buffer, result);
                                    return true;
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Timeout - ignore
                        }
                    }
                    catch (SocketException ex) when (IsAccessDenied(ex))
                    {
                        result.AppendLine("Error: Access denied. Please run with administrator privileges.");
                    }
                    catch (SocketException ex) when (IsNetworkUnreachable(ex))
                    {
                        // Network unreachable - ignore
                    }
                    catch (Exception ex)
                    {
                        result.AppendLine($"Error scanning {address}: {ex.Message}");
                    }

                    return false;
                }

                private static void ConfigureSocket(Socket socket)
                {
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment, false);
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 64);
                    socket.ReceiveTimeout = TIMEOUT_MS;
                    socket.SendTimeout = TIMEOUT_MS;
                }

                private static async Task ProcessSuccessfulResponse(IPAddress address, byte[] buffer, StringBuilder result)
                {
                    string hostName = await ICMP.GetHostNameAsync(address);
                    string os = ICMP.OsDetection(buffer[8]);
                    result.AppendLine($"Fragmented ICMP: Host {address} is alive. Hostname: {hostName}");
                    result.AppendLine($"      Operating System: {os}");
                    result.AppendLine($"      Response TTL: {buffer[8]}");
                    result.AppendLine();
                }

                private static byte[] CreateFragmentedIcmpPacket()
                {
                    byte[] packet = new byte[PACKET_SIZE];
                    packet[0] = 8;  // Type: Echo Request
                    packet[1] = 0;  // Code: 0
                    packet[2] = 0;  // Checksum (placeholder)
                    packet[3] = 0;  // Checksum (placeholder)

                    // Generate random identifier and sequence number
                    var random = new Random();
                    ushort identifier = (ushort)random.Next(0, ushort.MaxValue);
                    ushort sequence = (ushort)random.Next(0, ushort.MaxValue);

                    packet[4] = (byte)(identifier >> 8);    // Identifier high byte
                    packet[5] = (byte)(identifier & 0xFF);  // Identifier low byte
                    packet[6] = (byte)(sequence >> 8);      // Sequence high byte
                    packet[7] = (byte)(sequence & 0xFF);    // Sequence low byte

                    // Fill the rest with a pattern
                    for (int i = 8; i < packet.Length; i++)
                    {
                        packet[i] = (byte)(i % 256);
                    }

                    // Calculate checksum
                    ushort checksum = Utils.CalculateChecksum(packet);
                    packet[2] = (byte)(checksum >> 8);
                    packet[3] = (byte)(checksum & 255);

                    return packet;
                }

                private static bool IsAdministrator()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                    var principal = new System.Security.Principal.WindowsPrincipal(identity);
                    return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                }
                else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                {
                    // For Unix-like systems, check if we're root (UID 0)
                    return Environment.UserName == "root" || Environment.GetEnvironmentVariable("SUDO_USER") != null;
                }

                return false; // Unknown platform
            }
            catch
            {
                return false;
            }
        }

        private static bool IsAccessDenied(SocketException ex) =>
                    ex.SocketErrorCode == SocketError.AccessDenied;

                private static bool IsNetworkUnreachable(SocketException ex) =>
                    ex.SocketErrorCode == SocketError.NetworkUnreachable;
            }
        }