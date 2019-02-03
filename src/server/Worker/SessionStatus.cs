using Atropos.Server.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Worker
{
	public class SessionStatus
	{
		public SessionData GetCurrent()
		{
			var sessionId = SessionInformation.GetSession();
			if (sessionId < SessionInformation.NoSession)
			{
				var reason = default(SessionChangeReason);
				var sd = new SessionData(sessionId, reason.ToCode().ToKind(), this);
				sd.User = SessionInformation.GetUsernameBySessionId(sessionId, false);

				var state = SessionInformation.GetSessionLockState(sessionId);
				sd.IsLocked = state == SessionInformation.LockState.Locked;

				return sd;
			}
			return null;

		}
	}
}
