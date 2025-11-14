        using System.Net.NetworkInformation;
        using System.Net.Sockets;

        namespace gradproject
        {
            public static class NetworkInterfaceManager
            {
                public static async Task<NetworkInterface?> SelectNetworkInterfaceAsync()
                {
                    return await Task.Run(() =>
                    {
                        var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                            .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                            .ToList();

                        if (!interfaces.Any())
                        {
                            Console.WriteLine("No active network interfaces found.");
                            return null;
                        }

                        Console.WriteLine("\nAvailable Network Interfaces:");
                        for (int i = 0; i < interfaces.Count; i++)
                        {
                            var ni = interfaces[i];
                            var ipProps = ni.GetIPProperties();
                            var ipv4 = ipProps.UnicastAddresses
                                .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);

                            Console.WriteLine($"{i + 1}. {ni.Name}");
                            Console.WriteLine($"   Description: {ni.Description}");
                            Console.WriteLine($"   IP Address: {ipv4?.Address}");
                            Console.WriteLine($"   Speed: {ni.Speed / 1000000} Mbps");
                            Console.WriteLine();
                        }

                        while (true)
                        {
                            Console.Write("Select interface number (or 0 to cancel): ");
                            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= interfaces.Count)
                            {
                                if (choice == 0) return null;
                                return interfaces[choice - 1];
                            }
                            Console.WriteLine("Invalid selection. Please try again.");
                        }
                    });
                }
            }
        }