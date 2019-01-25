using Atropos.Common.Logging;
using Atropos.Server.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server
{
	public class Accounter
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public void Changed(SessionData data)
		{
			//var state = SessionInformation.GetWTSConnectState(data.SessionID);
			var state = SessionInformation.GetSessionLockState(data.SessionID);
			Log.DebugFormat("session changed {0}, state:{1}", data, state);
		}
	}
}
