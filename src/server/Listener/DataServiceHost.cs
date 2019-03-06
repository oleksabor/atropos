using Atropos.Common;
using Atropos.Common.Dto;
using Atropos.Server.Factory;
using com.Tools.WcfHosting;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Listener
{
	public class DataServiceHost : DisposeGently
	{
		protected readonly Grpc.Core.Server server;

		public DataServiceHost(DataServiceImpl service) 
		{
			server = new Grpc.Core.Server
			{
				Services = { DataService.BindService(service) },
				Ports = { new ServerPort("localhost", 12345, ServerCredentials.Insecure) }
			};
		}
		protected object startingLock = new object();
		protected bool started;

		public void Start()
		{
			Do(() => !started, () =>
			{
				server.Start();
				started = true;
			});
		}

		public void Stop()
		{
			Do(() => started, () => 
			{
				server.ShutdownAsync().Wait();
				started = false;
			});
		}

		void Do(Func<bool> canProceed, Action action)
		{
			if (canProceed())
				lock (startingLock)
					if (canProceed())
						action();
		}

		public override void DisposeIt()
		{
			Stop();
		}
	}
}
