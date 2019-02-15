using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting
{
	/// <summary>
	/// additional abstraction that can be used for remote service access. Hides WcfHosting from the user
	/// </summary>
	/// <typeparam name="TService">The type of the remote service.</typeparam>
	public class RemoteAccess<TService>
	{
		private readonly Func<TService> service;
		private readonly Action disconnect;

		/// <summary>
		/// Initializes a new instance of the <see cref="RemoteAccess{TService}"/> class.
		/// </summary>
		/// <param name="service">The service factory.</param>
		/// <param name="disconnect">Is executed in case of remote access failure.</param>
		public RemoteAccess(Func<TService> service, Action disconnect)
		{
			this.service = service;
			this.disconnect = disconnect;
		}

		public void Perform(Action<TService> run)
		{
			try
			{
				run(service());
			}
			catch (Exception)
			{
				DisconnectQuiet();
				throw;
			}
		}

		public TResult Perform<TResult>(Func<TService, TResult> run)
		{
			try
			{
				return run(service());
			}
			catch (Exception)
			{
				DisconnectQuiet();
				throw;
			}
		}

		void DisconnectQuiet()
		{
			try
			{
				disconnect();
			}
			catch (Exception)
			{ }
		}

		/// <summary>
		/// Checks if the remote service is ready.
		/// </summary>
		/// <param name="check">The check action to be executed.</param>
		/// <param name="attempts">How many time to execute an action if it fails</param>
		/// <param name="milliseconds2sleep">How long to sleep between action execution attempts.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// attempts - allowed parameter value is between 1 and 100
		/// or
		/// milliseconds2sleep - allowed parameter value is between 10 and 5000
		/// </exception>
		public void CheckIsRemoteReady(Action<TService> check, int attempts = 10, int milliseconds2sleep = 500)
		{
			if (attempts < 1 || attempts > 100)
				throw new ArgumentOutOfRangeException(nameof(attempts), "allowed parameter value is between 1 and 100");
			if (milliseconds2sleep < 10 || milliseconds2sleep > 5000)
				throw new ArgumentOutOfRangeException(nameof(milliseconds2sleep), "allowed parameter value is between 10 and 5000");
			var count = 0;
			while (count < attempts)
			{
				try
				{
					check(service());
					return;
				}
				catch (Exception)
				{
					DisconnectQuiet();
					count++;
					if (count >= attempts)
						throw;
					Thread.Sleep(milliseconds2sleep);
				}
			}
		}
	}
}
