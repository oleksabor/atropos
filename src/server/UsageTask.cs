using Atropos.Server.Db;
using Atropos.Server.Event;
using Atropos.Server.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server
{
	public class UsageTask : DisposeGently
	{
		Storage _storage;
		public UsageTask(Storage storage)
		{
			_storage = storage;
		}

		public void Store(SessionData data)
		{
			var today = DateTime.Today;

			var usage = _storage.GetUsage(data.User, today);
			switch (data.Reason)
			{
				case Kind.Connected:
					//_storage.AddUsage(data.User, TimeSpan.Zero, today);
					break;
				case Kind.Unknown:
				case Kind.Disconnected:
					if (data.User != "SYSTEM")
						_storage.AddUsage(data.User, data.Spent, today);
					break;

			}
		}

		public override void DisposeIt()
		{
			_storage.Dispose();
		}
	}
}
