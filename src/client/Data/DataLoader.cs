using Atropos.Common;
using Atropos.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Data
{
	public class DataLoader : PropertyChangedBase
	{
		public DataLoader(IDataService service)
		{
			Service = service;
			Users = new DataItems<User>(() => Service.GetUsers());
			UsageLog = new DataItem<UsageLog>(() => Service.GetUsageLog(SelectedUser?.Login, Date));
			Curfews = new DataItems<Curfew>(() => Service.GetCurfews(SelectedUser?.Login));
		}
		public DateTime Date { get; set; }

		public DataItems<User> Users { get; set; }

		public DataItem<UsageLog> UsageLog { get; set; }

		public DataItems<Curfew> Curfews { get; set; }

		private User _user;
		public User SelectedUser { get { return _user; } set { Set(ref _user, value); } }

		public IDataService Service { get; }

	}
}
