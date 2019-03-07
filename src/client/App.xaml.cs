using Atropos.Common;
using Atropos.Common.Logging;
using client.Data;
using Grpc.Core;
using Hardcodet.Wpf.TaskbarNotification;
using Nerdle.AutoConfig;
using System;
using System.Collections.Generic;
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

		DataLoader GetLoader()
		{
			var config = AutoConfig.Map<Settings>();
			if (config.Connection == null)
				throw new NullReferenceException("no Connection element");
			var remote = new DataServiceRemote(config.Connection);
			var dataLoader = new DataLoader(remote);
			dataLoader.Users.LoadAsync().ContinueWith(CheckTaskStatus);
			return dataLoader;
		}

		void CheckTaskStatus(Task task)
		{
			if (task.Exception != null)
				Log.ErrorException("failed to execute task", task.Exception);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
			base.OnExit(e);
		}
	}
}
