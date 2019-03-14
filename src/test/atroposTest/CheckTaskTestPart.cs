using Atropos.Common;
using Atropos.Server.Db;
using Atropos.Server.Worker;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposTest.Worker
{
	public partial class CheckTaskTest
	{
		[TestCase]
		public void CheckBreakSingleViolation()
		{
			var ct = new CheckTask(GetStorage());

			var usages = new[]
			{
				new UsageLog(new TimeSpan(9, 0, 0), TimeSpan.FromMinutes(101)),
			};

			var res = ct.CheckKind(new CheckParameter(TimeSpan.FromMinutes(200), TimeSpan.FromMinutes(30), usages));
			Assert.AreEqual(UsageResultKind.BreakRequired, res);
		}

		[TestCase]
		public void CheckBreakSingleAllowed()
		{
			var ct = new CheckTask(GetStorage());

			var usages = new[]
			{
				new UsageLog(new TimeSpan(9, 0, 0), TimeSpan.FromMinutes(100)),
			};

			var res = ct.CheckKind(new CheckParameter(200, 30, usages));
			Assert.AreEqual(UsageResultKind.NoRestriction, res);
		}

		[TestCase]
		public void CheckBreakSeveral()
		{
			var ct = new CheckTask(GetStorage());

			var usages = new List<UsageLog>
			{
				new UsageLog(new TimeSpan(9, 0, 0), TimeSpan.FromMinutes(50)),
				new UsageLog(new TimeSpan(10, 0, 0), TimeSpan.FromMinutes(50)),
			};

			var res = ct.CheckKind(new CheckParameter(200, 30, usages));
			Assert.AreEqual(UsageResultKind.NoRestriction, res);

			usages.Add(new UsageLog(new TimeSpan(11, 0, 0), TimeSpan.FromMinutes(1)));

			res = ct.CheckKind(new CheckParameter(200, 30, usages));
			Assert.AreEqual(UsageResultKind.BreakRequired, res);
		}

		[TestCase]
		public void CheckBreakPassedAllowed()
		{
			var ct = new CheckTask(GetStorage());

			var usages = new List<UsageLog>
			{
				new UsageLog(new TimeSpan(9, 0, 0), TimeSpan.FromMinutes(50)),
				new UsageLog(new TimeSpan(10, 0, 0), TimeSpan.FromMinutes(40)),

				new UsageLog(new TimeSpan(16, 0, 0), TimeSpan.FromMinutes(50)),
			};

			var res = ct.CheckKind(new CheckParameter(200, 30, usages));
			Assert.AreEqual(UsageResultKind.NoRestriction, res);
		}

		[TestCase]
		public void CheckBreakPassedViolated()
		{
			var ct = new CheckTask(GetStorage());

			var usages = new List<UsageLog>
			{
				new UsageLog(new TimeSpan(9, 0, 0), TimeSpan.FromMinutes(50)),
				new UsageLog(new TimeSpan(10, 0, 0), TimeSpan.FromMinutes(40)),

				new UsageLog(new TimeSpan(16, 0, 0), TimeSpan.FromMinutes(50)),
			};

			var res = ct.CheckKind(new CheckParameter(200, 30, usages));
			Assert.AreEqual(UsageResultKind.NoRestriction, res);
		}
	}
}
