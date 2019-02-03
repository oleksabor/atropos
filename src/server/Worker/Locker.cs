using Atropos.Common;
using Atropos.Common.Logging;
using Atropos.Common.String;
using Atropos.Server.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Worker
{
	/// <summary>
	/// checks database UsageLog periodically and locks user if curfew is exceeded
	/// </summary>
	/// <seealso cref="Atropos.Server.Factory.BackgroundTask" />
	public class Locker : BackgroundTask
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();
		Instance _instance;

		public SessionStatus State { get; }

		public Locker(Instance factory, SessionStatus state)
		{
			_instance = factory;
			State = state;
		}

		public override void Start()
		{
			base.Start(30);
		}

		DayOfWeek LoggedBlocked;
		string LoggedUser;

		public override void Run()
		{
			var sd = State.GetCurrent();
			if (sd == null || sd.User.IsEmpty() || sd.IsLocked)
				return;

			using (var c = _instance.Child())
			{
				var checkTask = c.Create<CheckTask>();
				var res = checkTask.Check(sd.User, DateTime.Today);
				switch (res.Kind)
				{
					case UsageResultKind.Blocked:
						var day = DateTime.Today.DayOfWeek;
						if ((LoggedBlocked == default(DayOfWeek) || LoggedBlocked != day) && !sd.User.Equals(LoggedUser, StringComparison.OrdinalIgnoreCase))
							Log.WarnFormat("{0} used computer for {1}", sd.User, res.Used);
						LoggedBlocked = day;
						LoggedUser = sd.User;
						break;
				}
			}
		}
	}
}
