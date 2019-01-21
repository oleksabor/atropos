﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Atropos.Server.Event
{
	public static class SessionChangeExtension
	{
		public static SessionChangeReasonCode ToCode(this SessionChangeReason value)
		{
			switch (value)
			{
				case SessionChangeReason.ConsoleConnect: return SessionChangeReasonCode.ConsoleConnect;
				case SessionChangeReason.ConsoleDisconnect: return SessionChangeReasonCode.ConsoleDisconnect;
				case SessionChangeReason.RemoteConnect: return SessionChangeReasonCode.RemoteConnect;
				case SessionChangeReason.RemoteDisconnect: return SessionChangeReasonCode.RemoteDisconnect;
				case SessionChangeReason.SessionLock: return SessionChangeReasonCode.SessionLock;
				case SessionChangeReason.SessionUnlock: return SessionChangeReasonCode.SessionUnlock;
				case SessionChangeReason.SessionLogoff: return SessionChangeReasonCode.SessionLogoff;
				case SessionChangeReason.SessionLogon: return SessionChangeReasonCode.SessionLogon;
				case SessionChangeReason.SessionRemoteControl: return SessionChangeReasonCode.SessionRemoteControl;

				default: return default(SessionChangeReasonCode);
			}
		}
	}
}