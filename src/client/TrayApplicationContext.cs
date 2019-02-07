using Atropos.Common;
using Atropos.Common.Logging;
using client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
	public class TrayApplicationContext : ApplicationContext
	{
		private NotifyIcon trayIcon;

		static ILog Log = LogProvider.GetCurrentClassLogger();

		public TrayApplicationContext(DataLoader service)
		{
			// Initialize Tray Icon
			trayIcon = new NotifyIcon()
			{
				Icon = Properties.Resources.scissors,
				ContextMenu = new ContextMenu(new [] {
					new MenuItem("Config", Config),
					new MenuItem("Exit", Exit),
					}),
				Visible = true
			};
			Service = service;
		}

		public DataLoader Service { get; }

		void Config(object sender, EventArgs e)
		{
			using (var f = new Form1(Service))
				f.ShowDialog();
		}

		void Exit(object sender, EventArgs e)
		{
			// Hide tray icon, otherwise it will remain shown until user mouses over it
			trayIcon.Visible = false;

			Application.Exit();
		}
	}
}
