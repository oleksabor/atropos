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
		public DataLoader Service;

		public IconViewModel(DataLoader loader)
		{
			Service = loader;
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

		public ICommand ShowWindowCommand
		{
			get
			{
				return new DelegateCommand
				{
					CanExecuteFunc = () => Application.Current.MainWindow == null,
					CommandAction = () =>
					{
						var window = new MainWindow();
						Application.Current.MainWindow = window;
						window.DataContext = Service;
						Application.Current.MainWindow.Show();
					}
				};
			}
		}

	}

	/// <summary>
	/// Simplistic delegate command for the demo.
	/// </summary>
	public class DelegateCommand : ICommand
	{
		public Action CommandAction { get; set; }
		public Func<bool> CanExecuteFunc { get; set; }

		public void Execute(object parameter)
		{
			CommandAction();
		}

		public bool CanExecute(object parameter)
		{
			return CanExecuteFunc == null || CanExecuteFunc();
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}
