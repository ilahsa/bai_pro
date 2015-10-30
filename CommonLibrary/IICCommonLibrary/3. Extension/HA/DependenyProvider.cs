using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.HA
{
	public class DependencyInfomation
	{
		public string Type;

		public string Path;

		public string Key;
	}

	public interface IDependencyProvider
	{
		List<DependencyInfomation> GetDependencyInfor();
	}

	public class DependenyWalker
	{
		public static void RegisterProvider()
		{
		}

		public static List<DependencyInfomation> WalkerThrough()
		{
			throw new NotImplementedException();
		}
	}
}
