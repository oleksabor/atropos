using Atropos.Common;
using Atropos.Common.Dto;
using client.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Data
{
	public class DataLoader : PropertyChangedBase
	{
		public DataLoader(Func<IDataService> service, Action disconnect)
		{
			Service = service;
			Disconnect = disconnect;
			Users = new DataItems<User>(LoadUsers);
			UsageLog = new DataItems<UsageLog>(() => DoOrDisconnect(() => Service().GetUsageLog(SelectedUser?.Login, Date)));
			Curfews = new DataItems<CurfewGui>(LoadCurfews);
			Date = DateTime.Today;
		}

		T DoOrDisconnect<T>(Func<T> a)
		{
			try
			{
				return a();
			}
			catch (Exception)
			{
				Disconnect();
				throw;
			}
		}

		IEnumerable<User> LoadUsers()
		{
			return DoOrDisconnect(() => Service().GetUsers());
		}

		IEnumerable<CurfewGui> LoadCurfews()
		{
			var curfews = DoOrDisconnect(() => Service().GetCurfews(SelectedUser?.Login));
			return curfews.Select(_ => new CurfewGui(_));
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

		public Func<IDataService> Service { get; }
		public Action Disconnect { get; }
	}
}
