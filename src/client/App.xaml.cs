using Atropos.Common.Logging;
using client.Data;
using Grpc.Core;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static Atropos.Common.Dto.DataService;

namespace client.Wpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private TaskbarIcon notifyIcon;

		static ILog Log = LogProvider.GetCurrentClassLogger();

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

		Channel channel;

		DataLoader GetLoader()
		{
			//var config = new CommunicationSettings { Host = new EndpointSettings { Uri = "net.pipe://localhost/atropos", Binding = "atropos_binding" } };

			channel = new Channel("localhost", 12345, ChannelCredentials.Insecure);
			var client = new DataServiceRemote(new DataServiceClient(channel));

			var dataLoader = new DataLoader(client);
			//remoteAccess.CheckIsRemoteReady(_ => dataLoader.Users.LoadAsync().Wait());
			CheckIsRemoteReady(() => dataLoader.Users.LoadAsync().Wait());
			return dataLoader;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			channel.ShutdownAsync().Wait();

			notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
			base.OnExit(e);
		}

		/// <summary>
		/// Checks if the remote service is ready.
		/// </summary>
		/// <param name="check">The check action to be executed.</param>
		/// <param name="attempts">How many time to execute an action if it fails</param>
		/// <param name="milliseconds2sleep">How long to sleep between action execution attempts.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// attempts - allowed parameter value is between 1 and 100
		/// or
		/// milliseconds2sleep - allowed parameter value is between 10 and 5000
		/// </exception>
		public void CheckIsRemoteReady(Action check, int attempts = 10, int milliseconds2sleep = 500)
		{
			if (attempts < 1 || attempts > 100)
				throw new ArgumentOutOfRangeException(nameof(attempts), "allowed parameter value is between 1 and 100");
			if (milliseconds2sleep < 10 || milliseconds2sleep > 5000)
				throw new ArgumentOutOfRangeException(nameof(milliseconds2sleep), "allowed parameter value is between 10 and 5000");
			var count = 0;
			while (count < attempts)
			{
				try
				{
					check();
					return;
				}
				catch (Exception)
				{
					count++;
					if (count >= attempts)
						throw;
					Thread.Sleep(milliseconds2sleep);
				}
			}
		}
	}
}
