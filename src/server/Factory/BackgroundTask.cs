using Atropos.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atropos.Server.Factory
{
	/// <summary>
	/// base class for running certain method in background thread
	/// </summary>
	public abstract class BackgroundTask
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		Task _sessionWatcher;
		CancellationTokenSource _cts;

		/// <summary>
		/// Gets or sets a value indicating whether method Run has to be executed when Stop is called.
		/// </summary>
		/// <value>
		///   <c>true</c> to execute Run(); otherwise, <c>false</c>.
		/// </value>
		public bool RunOnStop { get; protected set; }

		public abstract void Start();

		/// <summary>
		/// Starts to execute the <see cref="Run"/> method with pause between.
		/// </summary>
		/// <param name="pause">The pause time between method execution (in seconds).</param>
		public void Start(int pause)
		{
			DoSafe(_startLock, () => _sessionWatcher == null, () =>
			{
				_cts = new CancellationTokenSource();
				Log.DebugFormat("starting {0}", ClassName);
				_sessionWatcher = Task.Run(() => Watch(new Startup { CnclToken = _cts.Token, Pause = pause }), _cts.Token);
			});
		}

		public abstract void Run();

		/// <summary>
		/// Stops background running. Executes <see cref="Run"/> method if <see cref="RunOnStop"/> was set
		/// </summary>
		public void Stop()
		{
			if (RunOnStop)
				RunWithLock();

			Log.DebugFormat("stopping {0}", ClassName);
			if (_cts != null)
			{
				_cts.Cancel();
				Thread.Sleep(_waitTime);
			}

			DoSafe(_runLock, () => _sessionWatcher != null && _sessionWatcher.Status == TaskStatus.Running, () =>
			{
				try
				{
					Log.WarnFormat("seesion watcher still is active {0}", ClassName);
					_sessionWatcher.Wait(_waitTime);
				}
				catch (Exception e)
				{
					Log.ErrorException("error while stopping {0}", e, ClassName);
				}
			});
			_sessionWatcher = null;
		}

		string ClassName {  get { return GetType().Name; } }

		class Startup
		{
			public int Pause;
			public CancellationToken CnclToken;
		}

		protected bool Stopping => _cts.IsCancellationRequested;

		protected const int _waitTime = 100;

		protected bool Running { get; private set; }

		object _runLock = new object();
		object _startLock = new object();
		
		private void Watch(Startup data)
		{
			try
			{
				var cncl = data.CnclToken;

				while (!cncl.IsCancellationRequested)
				{
					RunWithLock();

					SleepAWhile(cncl, data.Pause, _waitTime);
				}
			}
			catch (Exception e)
			{
				Log.ErrorException("failed to watch {0}", e, ClassName);
			}
			Log.Trace("stopped watching");
		}

		protected void RunWithLock()
		{
			DoSafe(_runLock, () => !Running, () =>
				{
				try
				{
					Running = true;
					Run();
					errorCount = 0;
				}
				catch (Exception e)
				{
					Log.ErrorException($"run error {ClassName}", e);
					HandleFault?.Invoke(e, ++errorCount);
				}
				finally
				{
					Running = false;
				}
			});
		}

		protected void DoSafe(object locker, Func<bool> condition, Action work)
		{
			if (condition())
				lock (locker)
					if (condition())
						work();
		}

		protected void SleepAWhile(CancellationToken cncl, int waitTime, int sleepTime)
		{
			var time2sleep = waitTime * 1000 / sleepTime;
			for (int q = 0; q < time2sleep && !cncl.IsCancellationRequested; q++)
				Thread.Sleep(sleepTime);
		}

		/// <summary>
		/// Gets or sets the fault handler.
		/// </summary>
		/// <value>
		/// The fault handler.
		/// </value>
		public Action<Exception, int> HandleFault { get; set; }

		protected int errorCount { get; set; }
	}
}
