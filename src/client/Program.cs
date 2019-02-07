using Atropos.Common;
using Atropos.Common.Logging;
using client.Data;
using com.Tools.WcfHosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
	static class Program
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.ThreadException += Application_ThreadException;
			var config = new CommunicationSettings { Host = new EndpointSettings { Uri = "net.pipe://localhost/atropos", Binding = "atropos_binding" } };
			using (var dataService = new WcfClient<IDataService>(config))
			{
				dataService.Connect();
				var dataLoader = new DataLoader(dataService.Proxy);
				Application.Run(new TrayApplicationContext(dataLoader));
			}
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Log.ErrorException("unhandled exception", e.Exception);
		}
	}
}
