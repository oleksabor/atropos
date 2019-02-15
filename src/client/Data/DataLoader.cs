using Atropos.Common;
using Atropos.Common.Dto;
using client.Wpf;
using client.Wpf.Data;
using com.Tools.WcfHosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace client.Data
{
	public class DataLoader : PropertyChangedBase
	{
		public DataLoader(RemoteAccess<IDataService> service)
		{
			Users = new DataItems<User>(LoadUsers);
			UsageLog = new DataItems<UsageLog>(LoadUsageLog);
			Curfews = new DataItems<CurfewGui>(LoadCurfews);
			Date = DateTime.Today;
			Service = service;

			AddCurfew = new DelegateCommand() { CommandAction = () => AddCurfewMethod(), CanExecuteFunc = () => SelectedUser != null };
			DelCurfew = new DelegateCommand() { CommandAction = () => DelCurfewMethod(), CanExecuteFunc = () => SelectedCurfew != null };
			SaveCurfews = new DelegateCommand() { CommandAction = () => SaveCurfewsMethod(), CanExecuteFunc = () => SelectedUser != null };
			ReloadUsageLog = new DelegateCommand() { CommandAction = () => ReloadUsageLogMethod(), CanExecuteFunc = () => SelectedUser != null };
		}

		ObservableCollection<UsageLog> LoadUsageLog()
		{
			var usages = Service.Perform(_ => _.GetUsageLog(SelectedUser?.Login, Date));
			return new ObservableCollection<UsageLog>(usages);
		}

		ObservableCollection<User> LoadUsers()
		{
			var users = Service.Perform(_ => _.GetUsers());
			return new ObservableCollection<User>(users);
		}

		ObservableCollection<CurfewGui> LoadCurfews()
		{
			var curfews = Service.Perform(_ => _.GetCurfews(SelectedUser?.Login));
			return new ObservableCollection<CurfewGui>(curfews.Select(_ => _.ToGui()));
		}

		public DateTime Date { get; set; }

		public DataItems<User> Users { get; set; }

		public DataItems<UsageLog> UsageLog { get; set; }

		public DataItems<CurfewGui> Curfews { get; set; }

		CurfewGui _selectedCurfew;
		public CurfewGui SelectedCurfew { get { return _selectedCurfew; } set { Set(ref _selectedCurfew, value); } }

		public ICommand AddCurfew { get; protected set; }
		public void AddCurfewMethod()
		{
			var value = new CurfewGui();
			Curfews.Value.Add(value);
			SelectedCurfew = value;
		}

		public ICommand DelCurfew { get; protected set; }
		public void DelCurfewMethod()
		{
			Curfews.Value.Remove(SelectedCurfew);
			SelectedCurfew = null;
		}

		public ICommand SaveCurfews { get; protected set; }
		public void SaveCurfewsMethod()
		{
			var dtos = Curfews.Value.Select(_ => _.ToDto());
			Service.Perform(_ => _.SaveCurfew(dtos.ToArray(), SelectedUser.Login));
		}

		public ICommand ReloadUsageLog { get; protected set; }
		public void ReloadUsageLogMethod()
		{
			LoadCurfews();
		}

		private User _user;
		public User SelectedUser
		{
			get { return _user; }
			set
			{
				if (Set(ref _user, value))
				{
					Curfews.LoadAsync();
					UsageLog.LoadAsync();
				}
			}
		}

		public RemoteAccess<IDataService> Service { get; }
	}
}
