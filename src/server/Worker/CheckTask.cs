using Atropos.Common;
using Atropos.Common.Logging;
using Atropos.Common.String;
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
		static ILog Log = LogProvider.GetCurrentClassLogger();

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

			var day = (int)date.DayOfWeek;

			return Check(curFews, day, usage);
		}

		/// <summary>
		/// Checks the specified cufews. Minimal curfew Time allowed is taken if several curfews are defined for the same day (use comma and dash as separators)
		/// </summary>
		/// <param name="curFews">The curfews list.</param>
		/// <param name="day">The day to check.</param>
		/// <param name="usage">The usage log.</param>
		/// <returns>Usage result variable with Kind allowed to proceed or not</returns>
		public UsageResult Check(IEnumerable<Curfew> curFews, int day, IEnumerable<UsageLog> usages)
		{
			var curfew = curFews.OrderBy(_ => _.Time).FirstOrDefault(_ => _.WeekDay.IsWeekDay(day, Log));

			var cp = new CheckParameter(curfew?.Time, curfew?.Break, usages);

			var kind = curfew == null 
				? UsageResultKind.NoRestriction 
				: CheckKind(cp);

			return new UsageResult { Kind = kind, Used = TimeSpan.FromSeconds(cp.UsedSeconds) };
		}

		public UsageResultKind CheckKind(CheckParameter cp)
		{
			if (cp.AllowedTime < cp.UsedSeconds)
				return UsageResultKind.Blocked;

			// not enough data to understand how many time passed since user started to use the computer
			if (cp.BreakTime> 0)
			{
				UsageLog beforeBreak = null;
				var usedBeforeBreak = 0D;

				var usagesArray = cp.Usages.ToArray();
				for (int q = 0; q < usagesArray.Length - 1; q++)
				{
					var bb = usagesArray[q];
					var ab = usagesArray[q + 1];

					if (bb.Finished.TotalSeconds + cp.BreakTime * cp.BreakCorrector <= ab.Started.TotalSeconds)
					{
						beforeBreak = bb;
						usedBeforeBreak = cp.Usages.Where(_ => _.Started < ab.Started).Sum(_ => _.Used.TotalSeconds);
					}
				}

				if (beforeBreak == null && cp.UsedSeconds > cp.AllowedTime / 2)
					return UsageResultKind.BreakRequired;
			}
			return UsageResultKind.NoRestriction;
		}

		public override void DisposeIt()
		{
			_st.Dispose();
		}
	}
}
