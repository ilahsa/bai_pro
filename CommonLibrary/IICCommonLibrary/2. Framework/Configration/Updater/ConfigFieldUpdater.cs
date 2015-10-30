using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Configuration
{
	class ConfigFieldUpdater<T>: ConfigUpdater
	{
		private Action<T> _onUpdate;

		public ConfigFieldUpdater(string key, Action<T> onUpdate)
			: base(key, "", IICConfigType.Field)
		{
			_onUpdate = onUpdate;
		}

		public override void OnUpdate()
		{
			if (_onUpdate == null)
				throw new NotSupportedException("Not Support Update");

			IICConfigurationManager.Imp.GetConfigField<T>(p_path, _onUpdate);
		}
	}
}
