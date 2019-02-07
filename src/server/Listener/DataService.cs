using Atropos.Common;
using Atropos.Common.Dto;
using com.Tools.WcfHosting.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Listener
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	[InjectLogBehavior]
	public class DataService : IDataService
	{
		public DataService(Db.Storage storage)
		{
			Storage = storage;
			mapCurfew = new Mapper<Db.Curfew, Curfew>();
			mapUser = new Mapper<Db.User, User>();
			mapUsageLog = new Mapper<Db.UsageLog, UsageLog>();
		}

		public Db.Storage Storage { get; }

		private Mapper<Db.Curfew, Curfew> mapCurfew;
		private Mapper<Db.User, User> mapUser;
		private Mapper<Db.UsageLog, UsageLog> mapUsageLog;

		public Curfew[] GetCurfews(string login)
		{
			return mapCurfew.Map(Storage.GetUserCurfews(Storage.GetUser(login))).ToArray();
		}

		public UsageLog GetUsageLog(string login, DateTime date)
		{
			return mapUsageLog.Map(Storage.GetUsage(login, date));
		}

		public User[] GetUsers()
		{
			var users = Storage.GetUsers();
			return mapUser.Map(users).ToArray();
		}
	}
}
