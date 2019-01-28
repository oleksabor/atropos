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

namespace Atropos.Server
{
	public class Accounter : BackgroundTask, IDisposable
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();
		Instance _instance;

		public Accounter(Instance factory)
		{
			_instance = factory;
			_items = new ConcurrentQueue<SessionData>();
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

			Log.DebugFormat("session changed {0}, locked:{1}", data, data.IsLocked);

			Add(data);

			_watch.Value.Restart();
		}

		public void Add(SessionData data)
		{
			if ("SYSTEM".Equals(data.User, StringComparison.OrdinalIgnoreCase) || data.User.IsEmpty())
				return;
			_items.Enqueue(data);
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

		public void Start()
		{
			base.Start(10);
		}

		public override void Run()
		{
			Log.Trace("checking queue");
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
