using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Configuration
{
	public interface IConfigurationLoader
	{
		IICConfigFieldBuffer LoadConfigField(string key);

		IList<IICConfigItemBuffer> LoadConfigSection(string path);

		IList<IICConfigItemBuffer> LoadConfigItem(string path, string key);

		IICConfigTableBuffer LoadConfigTable(string tableName);

		string LoadConfigText(string path);
	}
}
