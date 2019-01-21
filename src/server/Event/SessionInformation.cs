using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Event
{
	public class SessionInformation
	{
		[DllImport("Wtsapi32.dll")]
		static extern bool WTSQuerySessionInformation(
			System.IntPtr hServer, uint sessionId, WtsInfoClass wtsInfoClass, out System.IntPtr ppBuffer, out uint pBytesReturned);

		// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/termserv/termserv/wtsgetactiveconsolesessionid.asp
		/// <summary>
		/// The WTSGetActiveConsoleSessionId function retrieves the 
		/// Terminal Services session currently attached to the physical console. 
		/// The physical console is the monitor, keyboard, and mouse.
		/// </summary>
		/// <returns>An <see cref="int"/> equal to 0 indicates that the current session is attached to the physical console.</returns>
		/// <remarks>It is not necessary that Terminal Services be running for this function to succeed.</remarks>
		[DllImport("Kernel32.dll")]
		public static extern uint WTSGetActiveConsoleSessionId();

		/// <summary>
		/// The WTSFreeMemory function frees memory allocated by a Terminal Services function.
		/// </summary>
		/// <param name="memory">Pointer to the memory to free.</param>
		[DllImport("wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
		public static extern void WTSFreeMemory(IntPtr memory);

		[DllImport("wtsapi32.dll")]
		private static extern void WTSFreeMemoryEx(
			WTS_TYPE_CLASS WTSTypeClass,
			IntPtr pMemory,
			UInt32 NumberOfEntries
		);

		private enum WTS_TYPE_CLASS
		{
			WTSTypeProcessInfoLevel0,
			WTSTypeProcessInfoLevel1,
			WTSTypeSessionInfoLevel1
		}

		public enum WTS_CONNECTSTATE_CLASS
		{
			Active,
			Connected,
			ConnectQuery,
			Shadow,
			Disconnected,
			Idle,
			Listen,
			Reset,
			Down,
			Init,
		}

		public const int WTS_CURRENT_SESSION = -1;

		/// <summary>
		/// Gets the username by session identifier.
		/// </summary>
		/// <param name="sessionId">The session identifier.</param>
		/// <param name="prependDomain">if set to <c>true</c> [prepend domain].</param>
		/// <returns>empty string if no session information was found</returns>
		public static string GetUsernameBySessionId(uint sessionId, bool prependDomain)
		{
			string username = GetWTS(sessionId, WtsInfoClass.WTSUserName, _ => Marshal.PtrToStringAnsi(_));
			if (prependDomain && !string.IsNullOrEmpty(username))
			{
				var domain = GetWTS(sessionId, WtsInfoClass.WTSDomainName, _ => Marshal.PtrToStringAnsi(_));
				username = string.Format("{0}\\{1}", domain, username);
			}
			return username;
		}

		public static WTS_CONNECTSTATE_CLASS GetWTSConnectState(uint sessionId)
		{
			var state = GetWTS(sessionId, WtsInfoClass.WTSConnectState, _ => Marshal.ReadInt32(_));
			try
			{
				return (WTS_CONNECTSTATE_CLASS)state;
			}
			catch (Exception e)
			{
				throw new ArgumentOutOfRangeException("state", state, "failed to convert to WTS_CONNECTSTATE_CLASS");
			}
		}

		static T GetWTS<T>(uint sessionId, WtsInfoClass kind, Func<IntPtr, T> getResult)
		{
			IntPtr buffer;
			uint strLen;
			T data = default(T);
			if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, kind, out buffer, out strLen) && strLen > 1)
			{
				data = getResult(buffer);
				WTSFreeMemory(buffer);
			}
			return data;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if the current ehmsas.exe is being run locally or in a Remote session.
		/// </summary>
		/// <returns>Returns true if the process is being executed locally or from a Terminal Server session.</returns>
		public static uint GetSession()
		{
			return WTSGetActiveConsoleSessionId();
		}

		public const uint NoSession = 0xFFFFFFFF;

		private const Int32 FALSE = 0;

		private static readonly IntPtr WTS_CURRENT_SERVER = IntPtr.Zero;

		private const Int32 WTS_SESSIONSTATE_LOCK = 0;
		private const Int32 WTS_SESSIONSTATE_UNLOCK = 1;

		private static bool _is_win7 = false;

		static SessionInformation()
		{
			var os_version = Environment.OSVersion;
			_is_win7 = (os_version.Platform == PlatformID.Win32NT && os_version.Version.Major == 6 && os_version.Version.Minor == 1);
		}

		public enum LockState
		{
			Unknown,
			Locked,
			Unlocked
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct WTSINFOEX
		{
			public UInt32 Level;
			public UInt32 Reserved; /* I have observed the Data field is pushed down by 4 bytes so i have added this field as padding. */
			public WTSINFOEX_LEVEL Data;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct WTSINFOEX_LEVEL
		{
			public WTSINFOEX_LEVEL1 WTSInfoExLevel1;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct WTSINFOEX_LEVEL1
		{
			public UInt32 SessionId;
			public WTS_CONNECTSTATE_CLASS SessionState;
			public Int32 SessionFlags;

			/* I can't figure out what the rest of the struct should look like but as i don't need anything past the SessionFlags i'm not going to. */

		}

		private enum WtsInfoClass
		{
			WTSInitialProgram = 0,
			WTSApplicationName = 1,
			WTSWorkingDirectory = 2,
			WTSOEMId = 3,
			WTSSessionId = 4,
			WTSUserName = 5,
			WTSWinStationName = 6,
			WTSDomainName = 7,
			WTSConnectState = 8,
			WTSClientBuildNumber = 9,
			WTSClientName = 10,
			WTSClientDirectory = 11,
			WTSClientProductId = 12,
			WTSClientHardwareId = 13,
			WTSClientAddress = 14,
			WTSClientDisplay = 15,
			WTSClientProtocolType = 16,
			WTSIdleTime = 17,
			WTSLogonTime = 18,
			WTSIncomingBytes = 19,
			WTSOutgoingBytes = 20,
			WTSIncomingFrames = 21,
			WTSOutgoingFrames = 22,
			WTSClientInfo = 23,
			WTSSessionInfo = 24,
			WTSSessionInfoEx = 25,
			WTSConfigInfo = 26,
			WTSValidationInfo = 27,
			WTSSessionAddressV4 = 28,
			WTSIsRemoteSession = 29
		}

		public static LockState GetSessionLockState(UInt32 session_id)
		{
			IntPtr ppBuffer;
			UInt32 pBytesReturned;

			var result = WTSQuerySessionInformation(
				WTS_CURRENT_SERVER,
				session_id,
				WtsInfoClass.WTSSessionInfoEx,
				out ppBuffer,
				out pBytesReturned
			);

			if (!result)
				return LockState.Unknown;

			var session_info_ex = Marshal.PtrToStructure<WTSINFOEX>(ppBuffer);

			if (session_info_ex.Level != 1)
				return LockState.Unknown;

			var lock_state = session_info_ex.Data.WTSInfoExLevel1.SessionFlags;
			WTSFreeMemoryEx(WTS_TYPE_CLASS.WTSTypeSessionInfoLevel1, ppBuffer, pBytesReturned);

			if (_is_win7)
			{
				/* Ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ee621019(v=vs.85).aspx
					* Windows Server 2008 R2 and Windows 7:  Due to a code defect, the usage of the WTS_SESSIONSTATE_LOCK
					* and WTS_SESSIONSTATE_UNLOCK flags is reversed. That is, WTS_SESSIONSTATE_LOCK indicates that the
					* session is unlocked, and WTS_SESSIONSTATE_UNLOCK indicates the session is locked.
					* */
				switch (lock_state)
				{
					case WTS_SESSIONSTATE_LOCK:
						return LockState.Unlocked;

					case WTS_SESSIONSTATE_UNLOCK:
						return LockState.Locked;

					default:
						return LockState.Unknown;
				}
			}
			else
			{
				switch (lock_state)
				{
					case WTS_SESSIONSTATE_LOCK:
						return LockState.Locked;

					case WTS_SESSIONSTATE_UNLOCK:
						return LockState.Unlocked;

					default:
						return LockState.Unknown;
				}
			}
		}
	}
}
