using Atropos.Common;
using Atropos.Server.Db;
using Atropos.Server.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Worker
{
	/// <summary>
	/// check user time logic
	/// </summary>
	/// <seealso cref="Atropos.Server.Factory.DisposeGently" />
	public class CheckTask : DisposeGently
	{
		Storage _st;

		public CheckTask(Storage st)
		{
			_st = st;
		}

		public UsageResult Check(string login, DateTime date)
		{
			var usage = _st.GetUsage(login, date);
			if (usage == null)
				return new UsageResult { Kind = UsageResultKind.NoRestriction };

			var user = _st.GetUser(login);
			var curFews = _st.GetUserCurfews(user);

			var day = date.DayOfWeek.ToString();

			return Check(curFews, day, usage);
		}

		public UsageResult Check(IEnumerable<Curfew> curFews, string day, UsageLog usage)
		{
			var curfew = curFews.FirstOrDefault(_ => _.WeekDay == day);
			if (curfew != null)
			{
				if (curfew.Time < usage.Used)
					return new UsageResult { Kind = UsageResultKind.NoRestriction, Used = usage.Used };

				// not enough data to understand how many time passed since user started to use the computer
				//if (curfew.Break > TimeSpan.Zero)
				//{
				//	var beforeBreak = curfew.Time.TotalSeconds / 2;
				//	if (usage.Used > TimeSpan.FromSeconds(beforeBreak))
				//		return UsageResult.Break;
				//}
			}
			return new UsageResult { Kind = UsageResultKind.NoRestriction, Used = usage.Used };
		}

		public override void DisposeIt()
		{
			_st.Dispose();
		}
	}
}
