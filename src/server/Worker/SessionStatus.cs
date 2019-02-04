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
		public SessionData GetCurrent(object sender)
		{
			var sessionId = SessionInformation.GetSession();
			if (sessionId < SessionInformation.NoSession)
			{
				var sd = new SessionData(sessionId, Kind.Unknown, sender);
				sd.User = SessionInformation.GetUsernameBySessionId(sessionId, false);

				var state = SessionInformation.GetSessionLockState(sessionId);
				sd.IsLocked = state == SessionInformation.LockState.Locked;
				if (sd.IsLocked)
					sd.Reason = Kind.Locked;
				else
					sd.Reason = Kind.Active;

				return sd;
			}
			return null;

		}
	}
}
