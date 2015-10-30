using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Configuration
{
	class ConfigTableUpdater<K, V>: ConfigUpdater where V: IICCodeTableItem
	{
		private Action<IICCodeTable<K, V>> _onUpdate;

		public ConfigTableUpdater(string table, Action<IICCodeTable<K, V>> onUpdate)
			: base(table, "", IICConfigType.Table)
		{
			_onUpdate = onUpdate;
		}

		public override void OnUpdate()
		{
			if (_onUpdate == null)
				throw new NotSupportedException("Not Support Update");

			IICCodeTable<K, V> table = IICConfigurationManager.Imp.GetCodeTable<K, V>(p_path, _onUpdate);
			p_Version = table.Version;
		}
	}
}
