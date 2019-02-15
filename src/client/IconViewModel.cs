using client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace client.Wpf
{
	public class IconViewModel
	{
		Func<DataLoader> Factory;

		public IconViewModel(Func<DataLoader> factory)
		{
			Factory = factory;
		}

		/// <summary>
		/// Shuts down the application.
		/// </summary>
		public ICommand ExitApplicationCommand
		{
			get
			{
				return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
			}
		}

		public void ShowWindow()
		{
			var window = new MainWindow();
			Application.Current.MainWindow = window;
			window.DataContext = Factory();
			Application.Current.MainWindow.Show();
		}

		public ICommand ShowWindowCommand
		{
			get
			{
				return new DelegateCommand
				{
					CanExecuteFunc = () => Application.Current.MainWindow == null,
					CommandAction = ShowWindow,
				};
			}
		}

	}
}
