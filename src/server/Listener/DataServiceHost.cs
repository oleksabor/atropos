using Atropos.Common;
using Atropos.Common.Dto;
using Atropos.Common.Logging;
using Atropos.Server.Factory;
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
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public DataServiceHost(DataServiceImpl service, Connection config) 
		{
			server = new Grpc.Core.Server
			{
				Services = { DataService.BindService(service) },
				Ports = { new ServerPort("localhost", config.Port, ServerCredentials.Insecure) }
			};
			Log.Info($"configured to listen on {config.Port}");
		}
		protected object startingLock = new object();
		protected bool started;

		public void Start()
		{
			Do(() => !started, () =>
			{
				server.Start();
				started = true;
				Log.Info($"gRPC server was started");
			});
		}

		public void Stop()
		{
			Do(() => started, () => 
			{
				server.ShutdownAsync().Wait();
				Log.Info($"gRPC server was stopped");
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
