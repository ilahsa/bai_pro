using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.DbAccess;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		数据库方法的异步回调
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class AsyncDbContext<T>
	{
		public Database Database = null;
		public AsyncDbCallback<T> dbCallBack = null;
		internal DatabaseOperationContext OperationContext = null;

		public T EndInvoke()
		{
			T result = default(T);
			try {
				result = dbCallBack.EndInvoke();
			} catch (Exception ex) {
				Database.OnException(OperationContext, ex);
				Console.WriteLine(ex);
				throw ex;
			} finally {
				Database.OnFinally(OperationContext);
			}
			return result;
		}
	}
}
