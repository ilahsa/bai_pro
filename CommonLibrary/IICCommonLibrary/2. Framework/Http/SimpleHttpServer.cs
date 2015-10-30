using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Http
{
	public class SimpleHttpServer
	{
		HttpListener listener = new HttpListener();

		public static SimpleHttpServer Create(int port)
		{
			throw new NotImplementedException();
		}

		public void RegisterHandler(string prefix, SimpleHttpHandler httpHandler)
		{
			listener = new HttpListener();
			// listener.Prefixes.Add("http://*
			 
		}

		public void RegisterVirtualPath(string vpath, string ppath)
		{
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}
	}
}
