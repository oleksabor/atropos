using Atropos.Common.Logging;
using Atropos.Server.Event;
using Atropos.Server.Factory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server
{
	public class Accounter : BackgroundTask, IDisposable
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();
		Instance _instance;

		bool _inactive;

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

			if (_inactive)
			{
				Log.Warn("inactive already set");
				return;
			}

			var state = SessionInformation.GetSessionLockState(data.SessionID);

			data.Spent = milliseconds;

			Log.DebugFormat("session changed {0}, state:{1}", data, state);

			Add(data);

			_watch.Value.Restart();
		}

		public void Add(SessionData data)
		{
			_items.Enqueue(data);
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
			SessionData data;
			using (var c = _instance.Child())
				using (var ut = c.Create<UsageTask>())
				while ((data = Get()) != null)
				{
					ut.Store(data);
				}
		}
	}
}
