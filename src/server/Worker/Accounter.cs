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

		public Accounter(Instance factory, MarkerBool marker)
		{
			_instance = factory;
			_items = new ConcurrentQueue<SessionData>();
			_marker = marker;

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

			if (Stopping)
			{
				Log.Warn("accounter is being stopped");
				return;
			}

			data.Spent = milliseconds;

			Add(data);

			_watch.Value.Restart();
		}

		public void Add(SessionData data)
		{
			var quit = false;
			if (data.IsLocked)
				quit = LogOnce(_marker, "loggerLocked", () => Log.TraceFormat("session is locked, ignoring. sender:{0}", data.Sender));
			if ("SYSTEM".Equals(data.User, StringComparison.OrdinalIgnoreCase) || data.User.IsEmpty())
				quit |= LogOnce(_marker, "systemLocked", () => Log.TraceFormat("no user data (system), ignoring. sender:{0}", data.Sender));

			if (quit)
				return;

			_marker.Clear();
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
			base.Start(10);
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
