﻿using Atropos.Common.Dto;
using Atropos.Common.Logging;
using Atropos.Server.Db;
using Atropos.Server.Event;
using Atropos.Server.Factory;
using Atropos.Server.Listener;
using Atropos.Server.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Atropos.Server
{
	/// <summary>
	/// service class to start all listeners and counters
	/// </summary>
	/// <seealso cref="Topshelf.ServiceControl" />
	/// <seealso cref="Topshelf.ServiceSessionChange" />
	public class ServiceImpl : ServiceControl, ServiceSessionChange
	{
		Accounter _accounter;
		StorageTool _stTool;

		IList<BackgroundTask> _tasks;

		static ILog Log = LogProvider.GetCurrentClassLogger();

		public DataServiceHost Host { get; }

		public ServiceImpl(Woodpecker listener, Accounter accounter, StorageTool stTool, Locker locker, DataServiceHost host)
		{
			listener.OnFound += data => _accounter.Changed(data);
			_accounter = accounter;
			_stTool = stTool;
			Host = host;
			_tasks = new List<BackgroundTask>() { listener, accounter, locker };
		}

		public bool Start(HostControl hostControl)
		{
			Log.DebugFormat("starting service");
			try
			{
				_stTool.CheckDb(); // TODO start in new thread to reduce Start execution time ?
				_accounter.HandleFault = (ex, count) => ThreadFault(ex, count, hostControl);

				DoAllTasks(_tasks, t =>	t.Start());

				Host.Start();
			}
			catch (Exception e)
			{
				Log.WarnException("failed to start", e);
				throw new ApplicationException("failed to start service", e);
			}
			Log.Trace("started");
			return true;
		}

		int threadsErrorCount;

		void ThreadFault(Exception e, int errorCount, HostControl hc)
		{
			if (errorCount > 5 || ++threadsErrorCount > 5)
			{
				Log.ErrorException($"too many errors:{errorCount} or threadErrors:{threadsErrorCount}, service is going to be stopped", e);
				hc.Stop(TopshelfExitCode.AbnormalExit);
			}
		}

		void DoAllTasks(IEnumerable<BackgroundTask> tasks, Action<BackgroundTask> dotask)
		{
			foreach (var t in tasks)
				try
				{
					dotask(t);
				}
				catch (Exception e)
				{
					throw new ApplicationException(string.Format("failed to do task {0}", t.GetType().Name), e);
				}
		}

		public bool Stop(HostControl hostControl)
		{
			Log.Debug("stopping");

			Host.Stop();

			DoAllTasks(_tasks, t => t.Stop());

			Log.Trace("stopped");
			return true;
		}

		public bool Pause(HostControl hostControl)
		{
			Log.Trace("pause");
			return Stop(hostControl);
		}

		public bool Continue(HostControl hostControl)
		{
			Log.Trace("continue");
			return Start(hostControl);
		}

		public void SessionChange(HostControl hostControl, SessionChangedArguments args)
		{
			var sid = (uint)args.SessionId;
			var kind = Kind.Unknown;

			switch (args.ReasonCode)
			{
				// Kind.Active to store latest active time span when user ends work
				case SessionChangeReasonCode.ConsoleDisconnect:
				case SessionChangeReasonCode.RemoteDisconnect:
				case SessionChangeReasonCode.SessionLock:
				case SessionChangeReasonCode.SessionLogoff:
					kind = Kind.Active;
					break;
				// Kind.Connected to reset Accounter and start watching for user time since connected state
				case SessionChangeReasonCode.ConsoleConnect:
				case SessionChangeReasonCode.RemoteConnect:
				case SessionChangeReasonCode.SessionLogon:
				case SessionChangeReasonCode.SessionUnlock:
					kind = Kind.Connected;
					break;
			}
			if (kind != Kind.Unknown)
				_accounter.Changed(new SessionData(sid, kind, this) { User = SessionInformation.GetUsernameBySessionId(sid, false) });
		}
	}
}
