using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Event
{
	public class ScreenBlock
	{
		[DllImport("user32.dll", SetLastError = true)]
		static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

		public bool LogOff(bool force)
		{
			uint flags = EWX_LOGOFF;
			if (force)
				flags = EWX_FORCE;
			return ExitWindowsEx(flags, SHTDN_REASON_MAJOR_SOFTWARE);
		}

		uint EWX_FORCE = 0x00000004;

		uint EWX_SHUTDOWN = 0x00000001;
		uint EWX_LOGOFF = 0;

		public bool Shutdown(bool force)
		{
			uint flags = EWX_SHUTDOWN;
			if (force)
				flags |= EWX_FORCE;
			return ExitWindowsEx(flags, SHTDN_REASON_MAJOR_SOFTWARE);
		}

		uint SHTDN_REASON_MAJOR_SOFTWARE = 0x00030000;


		[DllImport("wtsapi32.dll", SetLastError = true)]
		static extern bool WTSDisconnectSession(IntPtr hServer, uint sessionId, bool bWait);

		static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

		public virtual void Disconnect(uint sessionId)
		{
			if (!WTSDisconnectSession(WTS_CURRENT_SERVER_HANDLE, sessionId, false))
				throw new Win32Exception();
		}
	}
}
