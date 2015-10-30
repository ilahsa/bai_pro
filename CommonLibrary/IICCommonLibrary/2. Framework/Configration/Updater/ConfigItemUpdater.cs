using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Configuration
{
	class ConfigItemUpdater<T>: ConfigUpdater where T: IICConfigItem
	{
		private Action<T> _onUpdate;

		public ConfigItemUpdater(string path, string key, Action<T> onUpdate)
			: base(path, key, IICConfigType.Item)
		{
			_onUpdate = onUpdate;
		}

		public override void OnUpdate()
		{
			if (_onUpdate == null)
				throw new NotSupportedException("Not Support Update");

			IICConfigurationManager.Imp.GetConfigItem<T>(p_path, p_key, _onUpdate);
		}
	}
}
