using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Configuration
{
	class ConfigSectionUpdater<T>: ConfigUpdater where T: IICConfigSection
	{
		private Action<T> _onUpdate;

		public ConfigSectionUpdater(string path, Action<T> onUpdate)
			: base(path, string.Empty, IICConfigType.Section)
		{
			_onUpdate = onUpdate;
		}

		public override void OnUpdate()
		{
			if (_onUpdate == null)
				throw new NotSupportedException("Not Support Update");

			IICConfigurationManager.Imp.GetConfigSecion<T>(p_path, _onUpdate);
		}
	}
}
