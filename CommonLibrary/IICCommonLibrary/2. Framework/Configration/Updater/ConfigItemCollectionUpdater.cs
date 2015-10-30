using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Configuration
{
	class ConfigItemCollectinoUpdater<K, V>: ConfigUpdater where V: IICConfigItem
	{
		private Action<IICConfigItemCollection<K, V>> _onUpdate;

		public ConfigItemCollectinoUpdater(string path, Action<IICConfigItemCollection<K, V>> onUpdate)
			: base(path, string.Empty, IICConfigType.Item)
		{
			_onUpdate = onUpdate;
		}

		public override void OnUpdate()
		{
			if (_onUpdate == null)
				throw new NotSupportedException("Not Support Update");

			IICConfigurationManager.Imp.GetConfigItems<K, V>(p_path, _onUpdate);
		}
	}
}
