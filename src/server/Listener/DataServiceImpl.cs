using Atropos.Common;
using Atropos.Common.Dto;
using Atropos.Common.Logging;
using Atropos.Server.Factory;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static Atropos.Common.Dto.DataService;

namespace Atropos.Server.Listener
{
	public class DataServiceImpl : DataServiceBase
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public DataServiceImpl(IInstance factory)
		{
			mapCurfew = new Mapper<Db.Curfew, Curfew>();
			mapUser = new Mapper<Db.User, User>();
			mapUsageLog = new Mapper<Db.UsageLog, UsageLog>();
			InstanceFactory = factory;
		}

		public IInstance InstanceFactory { get; }

		private Mapper<Db.Curfew, Curfew> mapCurfew;
		private Mapper<Db.User, User> mapUser;
		private Mapper<Db.UsageLog, UsageLog> mapUsageLog;

		public override async Task GetUsers(global::Google.Protobuf.WellKnownTypes.Empty request, IServerStreamWriter<User> responseStream, ServerCallContext context)
		{
			var users = await GetUsersAsync();
			foreach (var u in users)
				await responseStream.WriteAsync(u);
		}

		public override async Task GetCurfews(CurfewRequest request, IServerStreamWriter<Curfew> responseStream, ServerCallContext context)
		{
			var curfews = await GetCurfewsAsync(request.Login);
			foreach (var cf in curfews)
				await responseStream.WriteAsync(cf);
		}

		public override async Task GetUsageLog(UsageLogRequest request, IServerStreamWriter<global::Atropos.Common.Dto.UsageLog> responseStream, ServerCallContext context)
		{
			var usageLog = await GetUsageLogAsync(request.Login, DateTime.FromBinary(request.DateValue));
			foreach (var ul in usageLog)
				await responseStream.WriteAsync(ul);
		}

		public override async Task<Google.Protobuf.WellKnownTypes.Empty> SaveCurfew(SaveCurfewRequest request, ServerCallContext context)
		{
			await SaveCurfewAsync(request.Values.ToArray(), request.Login);
			return new Google.Protobuf.WellKnownTypes.Empty();
		}

		public Task<Curfew[]> GetCurfewsAsync(string login)
		{
			return Task.Run(() => GetCurfews(Db(), login));
		}

		public Curfew[] GetCurfews(Db.Storage Db, string login)
		{
			using (Db)
			{
				var user = Db.GetUser(login);
				var curfews = user.Curfews;
				if (!curfews.Any())
					Log.WarnFormat("no user:{0} curfews where found", login);
				return mapCurfew.Map(curfews).ToArray();
			}
		}

		public Task<UsageLog[]> GetUsageLogAsync(string login, DateTime date)
		{
			return Task.Run(() => GetUsageLog(Db(), login, date));
		}

		public UsageLog[] GetUsageLog(Db.Storage Db, string login, DateTime date)
		{
			using (Db)
			{
				return mapUsageLog.Map(Db.GetUsage(login, date)).ToArray();
			}
		}

		public Task<User[]> GetUsersAsync()
		{
			return Task.Run(() => GetUsers(Db()));
		}

		public User[] GetUsers(Db.Storage Db)
		{
			using (Db)
			{
				var users = Db.GetUsers();
				var res = mapUser.Map(users).ToArray();
				return res;
			}
		}

		public Task SaveCurfewAsync(Curfew[] values, string login)
		{
			return Task.Run(() => SaveCurfew(Db(), values, login));
		}

		public void SaveCurfew(Db.Storage Db, Curfew[] values, string login)
		{
			using (Db)
			{
				var mapper = new Mapper<Curfew, Db.Curfew>();
				var dbcurfews = mapper.Map(values);

				using (var tr = Db.BeginTransaction())
				{
					Db.RemoveCurfews(login);
					Db.AddCurfews(login, dbcurfews);
					tr.Commit();
				}
			}
		}

		Db.Storage Db()
		{
			return InstanceFactory.Create<Db.Storage>();
		}

	}
}
