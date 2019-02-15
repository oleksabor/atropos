using Atropos.Common;
using Atropos.Common.Dto;
using Atropos.Common.Logging;
using Atropos.Server.Factory;
using com.Tools.WcfHosting.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Listener
{
	/// <summary>
	/// class methods are not thread safe. Please ensure that class instance is not shared between threads
	/// </summary>
	/// <seealso cref="Atropos.Server.Factory.DisposeGently" />
	/// <seealso cref="Atropos.Common.IDataService" />
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	[InjectLogBehavior]
	public class DataService : DisposeGently, IDataService
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public DataService() // for WCF hosting
		{ }

		public DataService(Db.Storage db) // this is not thread safe
		{
			mapCurfew = new Mapper<Db.Curfew, Curfew>();
			mapUser = new Mapper<Db.User, User>();
			mapUsageLog = new Mapper<Db.UsageLog, UsageLog>();
			Db = db;
		}

		public IInstance InstanceFactory { get; }
		public Db.Storage Db { get; }

		private Mapper<Db.Curfew, Curfew> mapCurfew;
		private Mapper<Db.User, User> mapUser;
		private Mapper<Db.UsageLog, UsageLog> mapUsageLog;

		public Curfew[] GetCurfews(string login)
		{
			var user = Db.GetUser(login);
			var curfews = user.Curfews;
			if (!curfews.Any())
				Log.WarnFormat("no user:{0} curfews where found", login);
			return mapCurfew.Map(curfews).ToArray();
		}

		public UsageLog[] GetUsageLog(string login, DateTime date)
		{
			return mapUsageLog.Map(Db.GetUsage(login, date)).ToArray();
		}

		public User[] GetUsers()
		{
			var users = Db.GetUsers();
			var res = mapUser.Map(users).ToArray();
			return res;
		}

		public override void DisposeIt()
		{
			Db.Dispose();
		}
	}
}
