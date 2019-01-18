using accountTimer.Event;
using accountTimer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace accountTimer
{
	public class ServiceImpl : ServiceControl, ServiceSessionChange
	{
		Woodpecker _listener;
		string _serviceName;
		Accounter _accounter;

		static ILog Log = LogProvider.GetCurrentClassLogger();

		public ServiceImpl(string serviceName, Woodpecker listener, Accounter accounter)
		{
			_listener = listener;
			_listener.OnFound += session_OnFound;
			_serviceName = serviceName;
			_accounter = accounter;
		}

		private void session_OnFound(SessionData data)
		{
			_accounter.Changed(data);
		}

		public bool Start(HostControl hostControl)
		{
			Log.DebugFormat("starting service:{0}", _serviceName);
			try
			{
				_listener.Start(_serviceName);
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
			Log.Trace("stopped");
			return true;
		}

		public void SessionChange(HostControl hostControl, SessionChangedArguments changedArguments)
		{
			var sid = (uint)changedArguments.SessionId;
			_accounter.Changed(new SessionData(sid, changedArguments.ReasonCode, this) { User = SessionInformation.GetUsernameBySessionId(sid, false) });
		}
	}
}
