using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class IPAddressHelper
	{
		private static IPAddress Loopback = IPAddress.Loopback;

		public static bool IsLocalNetwork(string ip)
		{
			return IsLocalNetwork(IPAddress.Parse(ip));
		}

		public static bool IsLocalNetwork(IPAddress ip)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork) {
				byte[] b = ip.GetAddressBytes();

				//	内网IP是以下面几个段开头的IP.用户可以自己设置.常用的内网IP地址: 
				//	10.x.x.x
				//	172.16.x.x至172.31.x.x
				//	192.168.x.x
				return IPAddress.IsLoopback(ip) ||
					IsSubnet(ip, IPAddress.Parse("10.0.0.0"), IPAddress.Parse("255.0.0.0")) ||
					IsSubnet(ip, IPAddress.Parse("172.16.0.0"), IPAddress.Parse("255.240.0.0")) ||
					IsSubnet(ip, IPAddress.Parse("192.168.0.0"), IPAddress.Parse("255.255.0.0"));

			} else {
				return false;
			}
		}

		public static bool IsSubnet(string ip, string subnet, string mask)
		{
			return IsSubnet(IPAddress.Parse(ip), IPAddress.Parse(subnet), IPAddress.Parse(mask));
		}

		public static bool IsSubnet(IPAddress ip, IPAddress subnet, IPAddress mask)
		{
			byte[] a = ip.GetAddressBytes();
			byte[] s = subnet.GetAddressBytes();
			byte[] m = mask.GetAddressBytes();

			bool r = true;
			for (int i = 0; i < a.Length; i++) {
				if ((a[i] & m[i]) != s[i]) {
					r = false;
				}
			}
			return r;
		}
	}
}
