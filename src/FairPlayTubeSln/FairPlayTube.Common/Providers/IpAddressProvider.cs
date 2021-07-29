using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace FairPlayTube.Common.Providers
{
    public class IpAddressProvider
    {
        public static List<string> GetCurrentHostIPv4Addresses(bool getPublicIpAddress = true)
        {
            if (getPublicIpAddress)
            {
                var publicIpAddress = GetPublicIp();
                return new List<string>() { publicIpAddress.ToString() };
            }
            //Check https://stackoverflow.com/questions/50386546/net-core-2-x-how-to-get-the-current-active-local-network-ipv4-address
            // order interfaces by speed and filter out down and loopback
            // take first of the remaining
            var allUpInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .OrderByDescending(c => c.Speed)
                .Where(c => c.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                c.OperationalStatus == OperationalStatus.Up).ToList();
            List<string> lstIps = new();
            if (allUpInterfaces != null && allUpInterfaces.Count > 0)
            {
                foreach (var singleUpInterface in allUpInterfaces)
                {
                    var props = singleUpInterface.GetIPProperties();
                    // get first IPV4 address assigned to this interface
                    var allIpV4Address = props.UnicastAddresses
                        .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select(c => c.Address)
                        .ToList();
                    allIpV4Address.ForEach((IpV4Address) =>
                    {
                        lstIps.Add(IpV4Address.ToString());
                    });
                }
            }

            return lstIps;
        }

        public static IPAddress GetPublicIp(string serviceUrl = "https://ipinfo.io/ip")
        {
            return IPAddress.Parse(new System.Net.WebClient().DownloadString(serviceUrl));
        }
    }
}
