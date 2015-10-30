using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		处理IICTracing的接口
	/// <seealso cref="TracingManager"/>
	/// <remarks>请从TracingManager.GetTracing系列方法获得一个可用的接口</remarks>
	/// </summary>
    public interface ITracing 
	{
		/// <summary>
		///		记录Info级别的日志
		/// </summary>
		/// <param name="message"></param>
        void Info(string message);

		/// <summary>
		///		记录Info级别的日志
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="message"></param>
		void Info(string from, string to, string message);

		/// <summary>
		///		记录Info级别的日志
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="message"></param>
        void Info(Exception exception, string message);

		/// <summary>
		///		记录Info级别的日志
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="message"></param>
		void Info(Exception exception, string from, string to, string message);

		/// <summary>
		///		记录Info级别的日志
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void InfoFmt(string format, params object[] args);

		/// <summary>
		///		记录Info级别的日志
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void InfoFmt(Exception exception, string format, params object[] args);

		/// <summary>
		///		记录Info级别的日志
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void InfoFmt2(string from, string to, string format, params object[] args);

		/// <summary>
		///		记录Info级别的日志
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void InfoFmt2(Exception exception, string from, string to, string format, params object[] args);

		/// <summary>
		///		记录Warn级别的日志
		/// </summary>
		void Warn(string message);
		void Warn(string from, string to, string message);
		void Warn(Exception exception, string message);
		void Warn(Exception exception, string from, string to, string message);
		void WarnFmt(string format, params object[] args);
		void WarnFmt(Exception exception, string format, params object[] args);
		void WarnFmt2(string from, string to, string format, params object[] args);
		void WarnFmt2(Exception exception, string from, string to, string format, params object[] args);

		void Error(string message);
		void Error(string from, string to, string message);
		void Error(Exception exception, string message);
		void Error(Exception exception, string from, string to, string message);
		void ErrorFmt(string format, params object[] args);
		void ErrorFmt(Exception exception, string format, params object[] args);
		void ErrorFmt2(string from, string to, string format, params object[] args);
		void ErrorFmt2(Exception exception, string from, string to, string format, params object[] args);

		void Info(Action callback);
		void Warn(Action callback);
		void Error(Action callback);

		string LoggerName { get; }
	}
}
