using Atropos.Server.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atropos.Server.Event
{
	public class Woodpecker
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		Task _sessionWatcher;
		CancellationTokenSource _cts;

		public void Start(string name)
		{
			_cts = new CancellationTokenSource();
			_sessionWatcher = new Task(Watch, _cts.Token, _cts.Token);
			_sessionWatcher.Start();
			//Log.Trace("running application");
			//var appThread = new Thread(() => Application.Run(new HiddenForm(name) { SessionAction = SessionAction }));
			//appThread.Start();
		}

		public void Stop()
		{
			//Log.Trace("exiting application");
			//Application.Exit();
			Log.Trace("cancelling token");
			_cts.Cancel();
			Thread.Sleep(_waitTime);

			if (_sessionWatcher.Status == TaskStatus.Running)
			{
				Log.Warn("seesion watcher still is active");
				_sessionWatcher.Wait(_waitTime / 2);
			}
		}

		const int _waitTime = 200;

		void Watch(object objCnclToken)
		{
			Log.TraceFormat("watching for current session waitTime:{0}", _waitTime);
			try
			{
				var cncl = (CancellationToken)objCnclToken;

				while (!cncl.IsCancellationRequested)
				{
					CheckSession();

					SleepAWhile(cncl, _waitTime);
				}
			}
			catch (Exception e)
			{
				Log.ErrorException("failed to watch for active session", e);
			}
			Log.Trace("stopped watching for current session");
		}

		void CheckSession()
		{
			var sessionId = SessionInformation.GetSession();
			if (sessionId < SessionInformation.NoSession)
				SessionAction(default(SessionChangeReason), sessionId);
		}

		void SleepAWhile(CancellationToken cncl, int waitTime)
		{
			var time2sleep = 1000 / waitTime * 30;
			for (int q = 0; q < time2sleep && !cncl.IsCancellationRequested; q++)
				Thread.Sleep(waitTime);
		}

		void SessionAction(SessionChangeReason reason, uint sessionID)
		{
			var sd = new SessionData(sessionID, reason.ToCode(), this);
			sd.User = SessionInformation.GetUsernameBySessionId(sessionID, false);

			OnFound?.Invoke(sd);
		}

		public event SessionFound OnFound;
	}

	public delegate void SessionFound(SessionData data);
}
