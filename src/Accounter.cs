using accountTimer.Event;
using accountTimer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accountTimer
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
