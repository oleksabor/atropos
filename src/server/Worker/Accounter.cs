using Atropos.Common.Logging;
using Atropos.Common.String;
using Atropos.Server.Event;
using Atropos.Server.Factory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Atropos.Server.Worker
{
	/// <summary>
	/// accepts session data events <see cref="Changed(SessionData)"/> and saves them to the database using <see cref="Run"/> method
	/// </summary>
	/// <seealso cref="Atropos.Server.Factory.BackgroundTask" />
	/// <seealso cref="System.IDisposable" />
	public class Accounter : BackgroundTask, IDisposable
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();
		Instance _instance;
		MarkerBool _marker;

		public Accounter(Instance factory, MarkerBool marker, Settings config)
		{
			_instance = factory;
			_items = new ConcurrentQueue<SessionData>();
			_marker = marker;
			Config = config;
			RunOnStop = true; // to save cached events
		}

		/// <summary>
		/// measures time portion between Changed method calls
		/// </summary>
		Lazy<Stopwatch> _watch = new Lazy<Stopwatch>(() => Stopwatch.StartNew(), true);

		ConcurrentQueue<SessionData> _items;

		public void Changed(SessionData data)
		{
			var milliseconds = TimeSpan.FromMilliseconds(_watch.Value.ElapsedMilliseconds);
			_watch.Value.Restart();

			if (Stopping)
			{
				Log.Warn("accounter is being stopped");
				return;
			}

			data.Spent = milliseconds;
			Add(data);
		}

		public void Add(SessionData data)
		{
			var quit = data.Spent == TimeSpan.Zero;
			if (data.Reason == Kind.Locked)
				quit |= LogOnce(_marker, "loggerLocked", () => Log.TraceFormat("session is locked, ignoring. sender:{0}", data.Sender));
			if (data.Reason == Kind.Connected)
			{
				quit |= true;
				Log.TraceFormat("session was connected, ignoring. sender:{0}", data.Sender);
			}

			if (!quit && data.User.IsEmpty())
				data.User = SessionInformation.GetUsernameBySessionId(data.SessionID, false);

			if ("SYSTEM".Equals(data.User, StringComparison.OrdinalIgnoreCase) || data.User.IsEmpty())
				quit |= LogOnce(_marker, "systemLocked", () => Log.TraceFormat("no user data (system), ignoring. sender:{0}", data.Sender));

			if (quit)
				return;

			_marker.Clear();
			if (data.Reason != Kind.Active) // like Connected
				Log.TraceFormat("adding session data reason:{0} spent:{1}", data.Reason, data.Spent);
			_items.Enqueue(data);
		}

		bool LogOnce(MarkerBool m, string key, Action log)
		{
			if (!m.Is(key))
			{
				log();
				m.Set(key, true);
			}
			return true;
		}

		bool Any()
		{
			return _items.Any();
		}

		public SessionData Get()
		{
			if (_items.TryDequeue(out SessionData res))
				return res;
			else
				return null;
		}

		private bool notDisposed = true;

		public Settings Config { get; }

		public void Dispose()
		{
			if (notDisposed)
			{
				notDisposed = false;
				_instance.Dispose();

				if (_watch.IsValueCreated)
					_watch.Value.Stop();
			}
		}

		public override void Start()
		{
			base.Start(Config.Interval.Accounter);
		}

		public override void Run()
		{
			SessionData data;
			if (Any())
			{
				using (var c = _instance.Child())
				using (var ut = c.Create<UsageTask>())
					while ((data = Get()) != null)
						ut.Store(data);
			}
		}
	}
}
