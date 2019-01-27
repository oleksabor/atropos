using Atropos.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atropos.Server.Factory
{
	public abstract class BackgroundTask
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		Task _sessionWatcher;
		CancellationTokenSource _cts;

		/// <summary>
		/// Starts to execute the <see cref="Run"/> method with pause between.
		/// </summary>
		/// <param name="pause">The pause time between method execution in seconds.</param>
		public void Start(int pause)
		{
			_cts = new CancellationTokenSource();
			_sessionWatcher = Task.Run(() => Watch(new Startup { CnclToken = _cts.Token, Pause = pause }), _cts.Token);
		}

		public abstract void Run();

		public void Stop()
		{
			Log.Trace("cancelling token");
			_cts.Cancel();
			Thread.Sleep(_waitTime);

			if (_sessionWatcher.Status == TaskStatus.Running)
			{
				Log.Warn("seesion watcher still is active");
				_sessionWatcher.Wait(_waitTime / 2);
			}
		}

		class Startup
		{
			public int Pause;
			public CancellationToken CnclToken;
		}

		protected const int _waitTime = 100;
		
		void Watch(object data)
		{
			try
			{
				var startup = (Startup)data;

				var cncl = startup.CnclToken;

				while (!cncl.IsCancellationRequested)
				{
					Run();

					SleepAWhile(cncl, startup.Pause, _waitTime);
				}
			}
			catch (Exception e)
			{
				Log.ErrorException("failed to watch for active session", e);
			}
			Log.Trace("stopped watching");
		}

		protected void SleepAWhile(CancellationToken cncl, int waitTime, int pauseBetween)
		{
			var time2sleep = 1000 / pauseBetween * waitTime;
			for (int q = 0; q < time2sleep && !cncl.IsCancellationRequested; q++)
				Thread.Sleep(waitTime);
		}
	}
}
