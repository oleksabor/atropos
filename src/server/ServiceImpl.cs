using Atropos.Common.Logging;
using Atropos.Server.Db;
using Atropos.Server.Event;
using Atropos.Server.Factory;
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
		ServiceOptions _options;
		Accounter _accounter;
		StorageTool _stTool;

		IList<BackgroundTask> _tasks;

		static ILog Log = LogProvider.GetCurrentClassLogger();

		public ServiceImpl(ServiceOptions options, Woodpecker listener, Accounter accounter, StorageTool stTool, Locker locker)
		{
			listener.OnFound += data => _accounter.Changed(data);
			_options = options;
			_accounter = accounter;
			_stTool = stTool;

			_tasks = new List<BackgroundTask>() { listener, accounter, locker };
		}

		public bool Start(HostControl hostControl)
		{
			Log.DebugFormat("starting service:{0}", _options.Name);
			try
			{
				_stTool.CheckDb(); // TODO start in new thread to reduce Start execution time ?

				DoAllTasks(_tasks, t =>	t.Start());
			}
			catch (Exception e)
			{
				Log.WarnException("failed to start", e);
				throw new ApplicationException("failed to start service", e);
			}
			Log.Trace("started");
			return true;
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

			DoAllTasks(_tasks, t => t.Stop());

			Log.Trace("stopped");
			return true;
		}

		public void SessionChange(HostControl hostControl, SessionChangedArguments changedArguments)
		{
			var sid = (uint)changedArguments.SessionId;
			_accounter.Changed(new SessionData(sid, changedArguments.ReasonCode.ToKind(), this) { User = SessionInformation.GetUsernameBySessionId(sid, false) });
		}
	}
}
