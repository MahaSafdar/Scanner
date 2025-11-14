using System;
using System.Net;

namespace gradproject
{
    static class Utils
    {
        public static uint IpToUint(IPAddress address)
        {
            byte[] bytes = address.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static IPAddress UintToIp(uint address)
        {
            byte[] bytes = BitConverter.GetBytes(address);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return new IPAddress(bytes);
        }

        public static ushort CalculateChecksum(byte[] packet)
        {
            int sum = 0;
            for (int i = 0; i < packet.Length - 1; i += 2)
            {
                sum += (packet[i] << 8) | packet[i + 1];
            }
            if (packet.Length % 2 == 1)
            {
                sum += packet[packet.Length - 1] << 8;
            }
            sum = (sum >> 16) + (sum & 0xffff);
            sum += sum >> 16;
            return (ushort)~sum;
        }
    }
}