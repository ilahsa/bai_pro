using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class Singleton<T>
	{
		private static object _syncRoot = new object();
		private static T _instance;

		public static T Instance
		{
			get 
			{
				if (_instance == null) {
					lock (_syncRoot) {
						if (_instance == null) {
							_instance = Activator.CreateInstance<T>();
						}
					}
				}
				return _instance; 
			}
		}
	}
}
