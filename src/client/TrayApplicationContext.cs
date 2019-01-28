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

		public TrayApplicationContext()
		{
			// Initialize Tray Icon
			trayIcon = new NotifyIcon()
			{
				Icon = Properties.Resources.scissors,
				ContextMenu = new ContextMenu(new MenuItem[] {
					new MenuItem("Exit", Exit)
					}),
				Visible = true
			};
		}

		void Exit(object sender, EventArgs e)
		{
			// Hide tray icon, otherwise it will remain shown until user mouses over it
			trayIcon.Visible = false;

			Application.Exit();
		}
	}
}
