using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Configuration
{
	class ConfigTextUpdater: ConfigUpdater
	{
		private Action<string> _onUpdate;

		public ConfigTextUpdater(string path, Action<string> onUpdate)
			: base(path, "", IICConfigType.Text)
		{
			_onUpdate = onUpdate;
		}

		public override void OnUpdate()
		{
			if (_onUpdate == null)
				throw new NotSupportedException("Not Support Update");

			IICConfigurationManager.Imp.GetConfigText(p_path, _onUpdate);
		}
	}
}
