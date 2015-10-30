/*
 * 负责按照类型生成唯一的RpcClientInterface, TypeSingletonFactory 模式
 * 
 * 2010-06-22
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	static class RpcClientInterfaceFactory<T>
	{
		private static object _syncRoot = new object();

		private static RpcClientInterface _instance = null;

		public static RpcClientInterface GetOne()
		{
			if (_instance == null) {
				lock (_syncRoot) {
					if (_instance == null) {
						_instance = new RpcClientInterface(typeof(T));
					}
				}
			}

			return  _instance;
		}
	}
}
