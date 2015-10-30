using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	public interface ITracingConsole
	{
		void AppendText(string text);
		void ResetColor();
		ConsoleColor TextColor { get; set; }
		ConsoleColor BackgroundColor { get; set; }
	}
}
