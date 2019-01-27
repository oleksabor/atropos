using Atropos.Common.Logging;
using Atropos.Server.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Atropos.Server.Event
{
	public class Woodpecker : BackgroundTask
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public void Start(string name)
		{
			base.Start(30);
		}

		public override void Run()
		{
			var sessionId = SessionInformation.GetSession();
			if (sessionId < SessionInformation.NoSession)
			{
				var reason = default(SessionChangeReason);
				var sd = new SessionData(sessionId, reason.ToCode().ToKind(), this);
				sd.User = SessionInformation.GetUsernameBySessionId(sessionId, false);

				var state = SessionInformation.GetSessionLockState(sessionId);
				sd.IsLocked = state == SessionInformation.LockState.Locked;

				OnFound?.Invoke(sd);
			}
		}

		public event SessionFound OnFound;
	}

	public delegate void SessionFound(SessionData data);
}
