using System;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.CommonV4
{
	public static class IICConfigurationManager
	{
		#region Private Static Fields
		private static IConfigurationLoader _loader = null;
		private static ConfiguratorProxy _proxy = null;
		private static ConfiguratorImp _imp = null;
		private static object _syncRoot = new object();
		#endregion

		#region Static Constructor
		static IICConfigurationManager()
		{
			_imp = new ConfiguratorImp();
			_proxy = new ConfiguratorProxy(_imp);
		}
		#endregion

		#region Public Properties & Config
		public static IConfigurator Configurator
		{
			get { return _proxy; }
		}

		public static IConfigurationLoader Loader
		{
			get { return _loader; }
			set { _loader = value; }
		}

		public static void UpdateConfig(string path, string key, IICConfigType configType)
		{
			_proxy.DoUpdate(path, key, configType);
		}
		#endregion

		#region Internal Fields
		internal static ConfiguratorProxy Proxy
		{
			get { return _proxy; }
		}

		internal static ConfiguratorImp Imp
		{
			get { return _imp; }
		}
		#endregion
	}
}
