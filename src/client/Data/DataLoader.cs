using Atropos.Common;
using Atropos.Common.Dto;
using client.Wpf.Data;
using com.Tools.WcfHosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			return new ObservableCollection<CurfewGui>(curfews.Select(_ => new CurfewGui(_)));
		}

		public DateTime Date { get; set; }

		public DataItems<User> Users { get; set; }

		public DataItems<UsageLog> UsageLog { get; set; }

		public DataItems<CurfewGui> Curfews { get; set; }

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
