using System;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Http
{
	public abstract class SimpleHttpHandler
	{
		public abstract void ProcessRequest(HttpWorkerRequest context);
	}
}
