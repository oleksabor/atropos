using Atropos.Server.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Worker
{
	public class CheckParameter
	{
		public double AllowedTime;
		public double BreakTime;
		public IEnumerable<UsageLog> Usages;

		public double UsedSeconds { get; }
		public double BreakCorrector { get; } = 0.8D;

		public CheckParameter()
		{ }

		public CheckParameter(int allowedMinutes, int breakMinutes, IEnumerable<UsageLog> usages)
			: this(TimeSpan.FromMinutes(allowedMinutes), TimeSpan.FromMinutes(breakMinutes), usages)
		{ }

		public CheckParameter(TimeSpan? allowedTime, TimeSpan? breakTime, IEnumerable<UsageLog> usages)
		{
			AllowedTime = ToDouble(allowedTime);
			BreakTime = ToDouble(breakTime);
			Usages = usages;

			UsedSeconds = Usages.Sum(_ => _.Used.TotalSeconds);


			double ToDouble(TimeSpan? value)
			{
				return value.HasValue ? value.Value.TotalSeconds : 0;
			}
		}
	}

}
