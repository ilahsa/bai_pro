using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Tracing
{
	[ProtoContract]
	public class SystemLogEvent
	{
		[ProtoMember(1)]
		[TableField("Time")]
		public DateTime Time;

		[ProtoMember(2)]
		[TableField("Level")]
		public TracingLevel Level;

		[ProtoMember(3)]
		[TableField("Service")]
		public string ServiceName;

		[ProtoMember(4)]
		[TableField("EventId", FieldType = typeof(string))]
		public LogEventID EventId;

		[ProtoMember(5)]
		[TableField("Message")]
		public string Message;

		[ProtoMember(6)]
		[TableField("Computer")]
		public string ComputerName;

		[ProtoMember(7)]
		[TableField("Repeat")]
		public int Repeat;

		public void WriteToConsole(string prompt, ITracingConsole console)
		{
			console.AppendText(Environment.NewLine);
			switch (Level) {
				case TracingLevel.Info:
					console.TextColor = ConsoleColor.Cyan;
					console.AppendText(prompt);
					console.AppendText("#<INFO > ");
					break;
				case TracingLevel.Warn:
					console.TextColor = ConsoleColor.Yellow;
					console.AppendText(prompt);
					console.AppendText("#<WARN > ");
					break;
				case TracingLevel.Error:
					console.TextColor = ConsoleColor.Red;
					console.AppendText(prompt);
					console.AppendText("#<ERROR> ");
					break;
			}

			console.AppendText(EventId.ToString());
			console.AppendText(": ");
			console.AppendText(XmlHelper.MaskInvalidCharacters(Message));
		}

		
		public void WriteToConsole(ITracingConsole console)
		{
			console.AppendText(Environment.NewLine);
			switch (Level) {
				case TracingLevel.Info:
					console.TextColor = ConsoleColor.Cyan;
					console.AppendText("<INFO > ");
					break;
				case TracingLevel.Warn:
					console.TextColor = ConsoleColor.Yellow;
					console.AppendText("<WARN > ");
					break;
				case TracingLevel.Error:
					console.TextColor = ConsoleColor.Red;
					console.AppendText("<ERROR> ");
					break;
			}

			console.AppendText(EventId.ToString());
			console.AppendText(": ");
			console.AppendText(XmlHelper.MaskInvalidCharacters(Message));
		}

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();

			str.Append("***\r\n===============================================================================\r\n");

			str.AppendFormat("{0} [{1}] SystemLog by <{2}> at \\\\{3} \r\n",
				Time.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				Level.ToString(),
				ServiceName,
				ComputerName);

			str.AppendFormat("Message: {0}\r\n",XmlHelper.MaskInvalidCharacters(Message));

			str.Append("===============================================================================\r\n");

			return str.ToString();		
		}
	}
}
