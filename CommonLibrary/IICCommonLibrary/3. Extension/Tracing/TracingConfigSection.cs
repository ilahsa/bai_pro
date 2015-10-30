using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	[IICConfigSection("IICTracing", IsRequired = false)]
	public class TracingConfigSection: IICConfigSection
	{
		[IICConfigField("Level")]
		public TracingLevel Level;

		[IICConfigField("AntiRepeat", DefaultValue = false)]
		public bool AntiRepeat;

		[IICConfigItemCollection("Appender")]
		public IICConfigItemCollection<AppenderType, TracingConfigAppenderItem> Appenders;

		public override void SetDefaultValue()
		{
			Level = TracingLevel.Off;
			Appenders = new IICConfigItemCollection<AppenderType, TracingConfigAppenderItem>();
		}
	}

	[IICConfigItem("Appender", KeyField = "Type", IsRequired = true)]
	public class TracingConfigAppenderItem: IICConfigItem
	{
		[IICConfigField("Type")]
		public AppenderType Type;

		[IICConfigField("Enabled")]
		public bool Enabled;

		[IICConfigField("Path", DefaultValue = "")]
		public string Path;
	}
}
