using Atropos.Common;
using Atropos.Common.Dto;
using Atropos.Common.Logging;
using Atropos.Server.Factory;
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
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public DataService(IInstance instanceFactory)
		{
			mapCurfew = new Mapper<Db.Curfew, Curfew>();
			mapUser = new Mapper<Db.User, User>();
			mapUsageLog = new Mapper<Db.UsageLog, UsageLog>();
			InstanceFactory = instanceFactory;
		}

		Db.Storage Storage()
		{
			return InstanceFactory.Create<Db.Storage>();
		}

		public IInstance InstanceFactory { get; }

		private Mapper<Db.Curfew, Curfew> mapCurfew;
		private Mapper<Db.User, User> mapUser;
		private Mapper<Db.UsageLog, UsageLog> mapUsageLog;

		public Curfew[] GetCurfews(string login)
		{
			using (var st = Storage())
			{
				var user = st.GetUser(login);
				var curfews = user.Curfews;
				if (!curfews.Any())
					Log.WarnFormat("no user:{0} curfews where found", login);
				return mapCurfew.Map(curfews).ToArray();
			}
		}

		public UsageLog[] GetUsageLog(string login, DateTime date)
		{
			using (var st = Storage())
				return mapUsageLog.Map(st.GetUsage(login, date)).ToArray();
		}

		public User[] GetUsers()
		{
			using (var st = Storage())
			{
				var users = st.GetUsers();
				var res = mapUser.Map(users).ToArray();
				return res;
			}
		}
	}
}
