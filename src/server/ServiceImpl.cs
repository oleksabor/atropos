using Atropos.Common.Logging;
using Atropos.Server.Db;
using Atropos.Server.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Atropos.Server
{
	public class ServiceImpl : ServiceControl, ServiceSessionChange
	{
		Woodpecker _listener;
		ServiceOptions _options;
		Accounter _accounter;
		StorageTool _stTool;

		static ILog Log = LogProvider.GetCurrentClassLogger();

		public ServiceImpl(ServiceOptions options, Woodpecker listener, Accounter accounter, StorageTool stTool)
		{
			_listener = listener;
			_listener.OnFound += session_OnFound;
			_options = options;
			_accounter = accounter;
			_stTool = stTool;
		}

		private void session_OnFound(SessionData data)
		{
			_accounter.Changed(data);
		}

		public bool Start(HostControl hostControl)
		{
			Log.DebugFormat("starting service:{0}", _options.Name);
			try
			{
				_stTool.CheckDb();
				_listener.Start(_options.Name);
				_accounter.Start();
			}
			catch (Exception e)
			{
				Log.WarnException("failed to start", e);
				throw new ApplicationException("failed to start service", e);
			}
			Log.Trace("started");
			return true;
		}

		public bool Stop(HostControl hostControl)
		{
			Log.Debug("stopping");
			_listener.Stop();
			_accounter.Stop();
			Log.Trace("stopped");
			return true;
		}

		public void SessionChange(HostControl hostControl, SessionChangedArguments changedArguments)
		{
			var sid = (uint)changedArguments.SessionId;
			_accounter.Changed(new SessionData(sid, changedArguments.ReasonCode.ToKind(), this) { User = SessionInformation.GetUsernameBySessionId(sid, false) });
		}
	}
}
