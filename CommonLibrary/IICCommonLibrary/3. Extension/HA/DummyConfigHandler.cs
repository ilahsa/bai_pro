using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	class DummyConfigHandler: ConfigurationSection
	{
		protected override void DeserializeSection(System.Xml.XmlReader reader)
		{
			// do nothing
		}
	}
}
