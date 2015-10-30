using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public abstract class HAComponent
	{
		public bool Running;

		public void DoStart()
		{

		}

		public abstract void Start();

		public abstract void Stop();

		public static void RunWithConsole<T>() where T: HAComponent
		{
			HAComponent component = Activator.CreateInstance<T>();

			try {
				Console.Write("Starting With Component:{0}...", ObjectHelper.GetTypeName(typeof(T), true));
				component.Start();
				Console.WriteLine("Done");

				while (true) {
					Console.Write(">");
					if (Console.ReadLine() == "stop") {
						return;
					}
				}
			} catch (Exception ex) {
				Console.WriteLine("Failed \r\n {0}", ex.ToString());
			}
		}
	}
}

