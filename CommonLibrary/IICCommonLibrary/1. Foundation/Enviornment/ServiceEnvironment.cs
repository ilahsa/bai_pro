using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		扩展的服务环境
	/// </summary>
	public static class ServiceEnvironment
	{
		#region Private Members
		private static string _serviceName;
		private static string _computerName;
		private static string _computerNameForConfig;
		private static string _computerAddress;
		private static string _processInfo;
		private static string _workPath;
		private static bool _debug;
		private static int _pid;
		#endregion

		#region Static Constructor
		static ServiceEnvironment()
		{
			Process process = Process.GetCurrentProcess();

			_serviceName = process.ProcessName;
			_computerName = Environment.MachineName;
			_computerNameForConfig = Environment.MachineName;
			_workPath = AppDomain.CurrentDomain.BaseDirectory;

			_pid = process.Id;
			_processInfo = string.Format("{0}-{1}", _pid, process.ProcessName);
			_debug = false;

			try {
				string host = Dns.GetHostName();
				var addrs = Dns.GetHostAddresses(host);
				foreach (var a in addrs) {
					if (a.AddressFamily == AddressFamily.InterNetwork) {
						_computerAddress = a.ToString();
					}
				}
			} catch (Exception) {
				_computerAddress = "0.0.0.0";
			}
		}
		#endregion

		#region Public Properties
		/// <summary>进程运行路径</summary>
		public static string WorkPath
		{
			get { return _workPath; }
		}

		/// <summary>进程Id</summary>
		public static int ProcessId
		{
			get { return _pid; }
		}

		/// <summary>进程消息</summary>
		public static string ProcessInfo
		{
			get { return _processInfo; }
		}

		/// <summary>服务名称</summary>
		public static string ServiceName
		{
			get { return _serviceName; }
			set { _serviceName = value; }
		}

		/// <summary>计算机名称</summary>
		public static string ComputerName
		{
			get { return _computerName; }
		}

		/// <summary>用于获取配置的计算机名称</summary>
		public static string ComputerNameForConfiguration
		{
			get { return _computerNameForConfig; }
			set { _computerNameForConfig = value; }
		}

		/// <summary>本机服务器地址, 自动获取</summary>
		public static string ComputerAddress
		{
			get { return _computerAddress; }			
		}

		/// <summary>是否允许Debug信息</summary>
		public static bool Debug
		{
			get { return _debug; }
			set { _debug = value; }
		}
		#endregion
	}
}
