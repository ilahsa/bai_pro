using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Tracing
{
	[ProtoContract]
	public class TracingEvent
	{
		[ProtoMember(1)]
		[TableField("Time")]
		public DateTime Time;

		[ProtoMember(2)]
		[TableField("Level", FieldType=typeof(int))]
		public TracingLevel Level;
		
		[ProtoMember(3)]
		[TableField("LoggerName")]
		public string LoggerName = string.Empty;
		
		[ProtoMember(4)]
		[TableField("Message")]
		public string Message = string.Empty;

		[ProtoMember(5)]
		[TableField("Error")]
		public string Error = string.Empty;

		[ProtoMember(6)]
		[TableField("Thread")]
		public string ThreadInfo = string.Empty;

		[ProtoMember(7)]
		[TableField("Process")]
		public string ProcessInfo = string.Empty;

		[ProtoMember(8)]
		[TableField("From")]
		public string From = string.Empty;

		[ProtoMember(9)]
		[TableField("To")]
		public string To = string.Empty;

		//[ProtoMember(10)]
		//[TableField("Session")]
		//public string Session;

		[ProtoMember(11)]
		[TableField("Computer")]
		public string ComputerName;

		[ProtoMember(12)]
		[TableField("Service")]
		public string ServiceName;

		[ProtoMember(13)]
		[TableField("Repeat")]
		public int Repeat = 0;

		public void WriteToConsole(ITracingConsole console)
		{
			console.AppendText(Environment.NewLine);
			switch (Level) {
				case TracingLevel.Info:
					console.TextColor = ConsoleColor.Gray;
					console.AppendText("[INFO ] ");
					break;
				case TracingLevel.Warn:
					console.TextColor = ConsoleColor.Yellow;
					console.AppendText("[WARN ] ");
					break;
				case TracingLevel.Error:
					console.TextColor = ConsoleColor.Red;
					console.AppendText("[ERROR] ");
					break;
			}

			console.AppendText(LoggerName);
			console.AppendText(": ");
			console.AppendText(XmlHelper.MaskInvalidCharacters(Message));

			if (!string.IsNullOrEmpty(Error)) {
				console.AppendText(Environment.NewLine);
				console.AppendText(XmlHelper.MaskInvalidCharacters(Error));
			}
		}

		public void WriteToConsole(string prompt, ITracingConsole console)
		{
			console.AppendText(Environment.NewLine);
			switch (Level) {
				case TracingLevel.Info:
					console.TextColor = ConsoleColor.Gray;
					console.AppendText(prompt);
					console.AppendText("#[INFO ] ");
					break;
				case TracingLevel.Warn:
					console.TextColor = ConsoleColor.Yellow;
					console.AppendText(prompt);
					console.AppendText("#on[WARN ] ");
					break;
				case TracingLevel.Error:
					console.TextColor = ConsoleColor.Red;
					console.AppendText(prompt);
					console.AppendText("#[ERROR] "); ;
					break;
			}

			console.AppendText(LoggerName);
			console.AppendText(": ");
			console.AppendText(XmlHelper.MaskInvalidCharacters(Message));

			if (!string.IsNullOrEmpty(Error)) {
				console.AppendText(Environment.NewLine);
				console.AppendText(XmlHelper.MaskInvalidCharacters(Error));
			}
		}


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("***\r\n-------------------------------------------------------------------------------\r\n");

			sb.AppendFormat("{0} [{1}] Tracing by <{2}> at \\\\{3} \r\n",
				Time.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				Level.ToString(),
				LoggerName,
				ComputerName);

			sb.AppendFormat(
				"Process: \"{0}\" Thread: \"{1}\" \r\n",
				ProcessInfo,
				ThreadInfo
			);

			sb.AppendFormat("Message: {0}\r\n", XmlHelper.MaskInvalidCharacters(Message));

			if (!string.IsNullOrEmpty(Error)) {
				sb.Append(Error);
			}

			if (!string.IsNullOrEmpty(From)) {
				sb.AppendFormat(
					"From: {0}\r\nTo: {1}\r\n",
					From, To
				);
			}

			sb.Append("-------------------------------------------------------------------------------\r\n");

			return sb.ToString();
		}
	}
}