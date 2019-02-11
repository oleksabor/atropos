using Atropos.Common;
using Atropos.Common.Logging;
using client.Data;
using client.Wpf;
using com.Tools.WcfHosting;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace client.Wpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private TaskbarIcon notifyIcon;

		static ILog Log = LogProvider.GetCurrentClassLogger();

		WcfClient<IDataService> DataService;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var config = new CommunicationSettings { Host = new EndpointSettings { Uri = "net.pipe://localhost/atropos", Binding = "atropos_binding" } };

			DataService = new WcfClient<IDataService>(config);
			DataService.Connect();
			var dataLoader = new DataLoader(DataService.Connect, DataService.Disconnect);

			Exception startError = null;
			var count = 0;
			while (count < 10)
			{
				var t = dataLoader.Users.LoadAsync();

				try
				{
					t.Wait();
					count += 50;
				}
				catch (Exception ex)
				{
					Log.WarnException("failed to get users list", ex);
					count++;
					Thread.Sleep(500);
					startError = ex;
				}
			}
			if (count < 50)
				throw new ApplicationException("failed to start", startError);

			//create the notifyicon (it's a resource declared in NotifyIconResources.xaml
			notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
			notifyIcon.DataContext = new IconViewModel(dataLoader);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			DataService.Dispose();
			notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
			base.OnExit(e);
		}
	}
}
