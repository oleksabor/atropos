using Atropos.Common.Logging;
using Atropos.Server.Db;
using Atropos.Server.Event;
using Atropos.Server.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Worker
{
	public class UsageTask : DisposeGently
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		Storage _storage;

		public UsageTask(Storage storage)
		{
			_storage = storage;
		}

		public void Store(SessionData data)
		{
			switch (data.Reason)
			{
				case Kind.Connected: // nothing to log here since user has just logged in
				case Kind.Locked:
				case Kind.Unknown: 
				case Kind.Disconnected: 
					break;

				case Kind.Active:
					Save(data);
					break;
			}
		}

		void Save(SessionData data)
		{
			var today = DateTime.Today;
			var usage = _storage.GetUsage(data.User, today);
			using (var t = _storage.BeginTransaction())
				try
				{
					Log.DebugFormat("saving usage {0} {1} reason:{2}", data.User, data.Spent.TotalSeconds, data.Reason);
					_storage.AddUsage(data.User, data.Spent, today);
					t.Commit();
				}
				catch (Exception e)
				{
					throw new ApplicationException(string.Format("failed to save usage data {0}", data), e);
				}
		}

		public override void DisposeIt()
		{
			_storage.Dispose();
		}
	}
}
