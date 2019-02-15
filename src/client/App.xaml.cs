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

			var iconVM = new IconViewModel(GetLoader);
			//create the notifyicon (it's a resource declared in NotifyIconResources.xaml
			notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
			notifyIcon.DataContext = iconVM;

			//opens MainWindows if started from VS
			if (System.Diagnostics.Debugger.IsAttached)
				iconVM.ShowWindowCommand.Execute(null);
		}

		DataLoader GetLoader()
		{
			var config = new CommunicationSettings { Host = new EndpointSettings { Uri = "net.pipe://localhost/atropos", Binding = "atropos_binding" } };

			DataService = new WcfClient<IDataService>(config);
			DataService.Connect();
			var remoteAccess = new RemoteAccess<IDataService>(DataService.Connect, DataService.Disconnect);
			var dataLoader = new DataLoader(remoteAccess);
			remoteAccess.CheckIsRemoteReady(_ => dataLoader.Users.LoadAsync().Wait());
			return dataLoader;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			DataService.Dispose();
			notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
			base.OnExit(e);
		}
	}
}
