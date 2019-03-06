using Atropos.Common;
using Atropos.Common.DateTimeConv;
using Atropos.Common.Dto;
using Atropos.Common.Logging;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Atropos.Common.Dto.DataService;

namespace client
{
	public class DataServiceRemote : IDataService
	{
		private readonly DataServiceClient client;
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public DataServiceRemote(DataServiceClient client)
		{
			this.client = client;
		}

		public User[] GetUsers()
		{
			try
			{
				var res = new Collection<User>();
				using (var call = client.GetUsers(new Empty()))
				{
					var rs = call.ResponseStream;

					while (rs.MoveNext().Result)
					{
						var user = rs.Current;
						res.Add(user);
					}
				}
				return res.ToArray();
			}
			catch (RpcException r)
			{
				Log.ErrorException("failed to get users", r);
				throw new ApplicationException("no users were loaded", r);
			}
		}

		public Curfew[] GetCurfews(string login)
		{
			var res = new Collection<Curfew>();
			using (var call = client.GetCurfews(new CurfewRequest { Login = login }))
			{
				var rs = call.ResponseStream;

				while (rs.MoveNext().Result)
				{
					var curfew = rs.Current;
					res.Add(curfew);
				}
			}
			return res.ToArray();
		}

		public UsageLog[] GetUsageLog(string login, DateTime date)
		{
			var res = new Collection<UsageLog>();
			using (var call = client.GetUsageLog(new UsageLogRequest { Login = login, DateValue = date.ToDto() }))
			{
				var rs = call.ResponseStream;

				while (rs.MoveNext().Result)
				{
					var value = rs.Current;
					res.Add(value);
				}
			}
			return res.ToArray();
		}

		public void SaveCurfew(Curfew[] values, string login)
		{
			var req = new SaveCurfewRequest { Login = login };
			req.Values.AddRange(values);

			client.SaveCurfew(req);
		}
	}
}
