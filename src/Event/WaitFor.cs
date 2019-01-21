using Atropos.Server.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Event
{
	class Wait
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		/// <summary>
		/// Waits for the specified event name .
		/// </summary>
		/// <param name="name">The event name to wait for.</param>
		/// <param name="timeout">The timeout in seconds.</param>
		public void ForEvent(string name, uint timeout)
		{
			IntPtr handle = OpenEvent(STANDARD_RIGHTS_REQUIRED, false, name);
			var error = Marshal.GetLastWin32Error();
			if (error != 0 || handle == IntPtr.Zero)
			{
				Log.WarnFormat("can't open event error:0x{0:x}", error);
			}
			else
			{
				if (timeout == 0)
					timeout = INFINITE;
				else
					timeout *= 1000;
				var waitResult = WaitForSingleObject(handle, timeout);
				if (waitResult != WAIT_OBJECT_0)
					Log.TraceFormat("failed to wait for '{0}', result is 0x{1:x}", name, waitResult);

				CloseHandle(handle);
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseHandle(IntPtr hObject);

		const UInt32 INFINITE = 0xFFFFFFFF;
		const UInt32 WAIT_ABANDONED = 0x00000080;
		const UInt32 WAIT_OBJECT_0 = 0x00000000;
		const UInt32 WAIT_TIMEOUT = 0x00000102;

		[DllImport("Kernel32.dll", SetLastError = true)]
		static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);

		// taken from header files
		const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
		const uint SYNCHRONIZE = 0x00100000;
		const uint EVENT_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x3);
		const uint EVENT_MODIFY_STATE = 0x0002;
		const long ERROR_FILE_NOT_FOUND = 2L;

		/// <summary>
		/// Security enumeration from:
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/synchronization_object_security_and_access_rights.asp
		/// </summary>
		[Flags]
		public enum SyncObjectAccess : uint
		{
			DELETE = 0x00010000,
			READ_CONTROL = 0x00020000,
			WRITE_DAC = 0x00040000,
			WRITE_OWNER = 0x00080000,
			SYNCHRONIZE = 0x00100000,
			EVENT_ALL_ACCESS = 0x001F0003,
			EVENT_MODIFY_STATE = 0x00000002,
			MUTEX_ALL_ACCESS = 0x001F0001,
			MUTEX_MODIFY_STATE = 0x00000001,
			SEMAPHORE_ALL_ACCESS = 0x001F0003,
			SEMAPHORE_MODIFY_STATE = 0x00000002,
			TIMER_ALL_ACCESS = 0x001F0003,
			TIMER_MODIFY_STATE = 0x00000002,
			TIMER_QUERY_STATE = 0x00000001
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
	}
}
