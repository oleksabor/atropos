using Atropos.Common;
using Atropos.Common.String;
using Atropos.Server.Db;
using Atropos.Server.Worker;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest.Worker
{
	[TestFixture]
	public class CheckTaskTest
	{
		[TestCase]
		public void CheckCurfewNoData()
		{
			var user = GetUser();
			var usageLog = GetUsageLog(user, TimeSpan.Zero);

			var curfews = GetCurfews(0, user, null);
			var st = GetStorage(user, usageLog, curfews);

			var c = new CheckTask(st);
			var res = c.Check(user.Login, usageLog.Date);

			Assert.AreEqual(UsageResultKind.NoRestriction, res.Kind);
		}

		[TestCase]
		public void CheckCurfewNoWeekDayMatch()
		{

			var user = GetUser();
			var usageLog = GetUsageLog(user, TimeSpan.FromHours(2));

			var weekDay = (int)usageLog.Date.AddDays(-1).DayOfWeek;

			var curfews = GetCurfews(1, user, (count, u) => new Curfew { Id = count, User = u, Time = TimeSpan.FromHours(1), WeekDay = weekDay.ToString() });
			var st = GetStorage(user, usageLog, curfews);

			var c = new CheckTask(st);
			var res = c.Check(user.Login, usageLog.Date);

			Assert.AreEqual(UsageResultKind.NoRestriction, res.Kind);
		}

		[TestCase]
		public void CheckCurfewWeekDayMatchUsedLessThanAllowed()
		{

			var user = GetUser();
			var usageLog = GetUsageLog(user, TimeSpan.FromMinutes(15));

			var weekDay = (int)usageLog.Date.DayOfWeek;

			var curfews = GetCurfews(1, user, (count, u) => new Curfew { Id = count, User = u, Time = TimeSpan.FromHours(1), WeekDay = weekDay.ToString() });
			var st = GetStorage(user, usageLog, curfews);

			var c = new CheckTask(st);
			var res = c.Check(user.Login, usageLog.Date);

			Assert.AreEqual(UsageResultKind.NoRestriction, res.Kind);
		}

		[TestCase]
		public void CheckCurfewWeekDayMatchUsedMoreThanAllowed()
		{

			var user = GetUser();
			var usageLog = GetUsageLog(user, TimeSpan.FromHours(2));

			var weekDay = (int)usageLog.Date.DayOfWeek;

			var curfews = GetCurfews(2, user, (count, u) => new Curfew { Id = count, User = u, Time = TimeSpan.FromHours(1), WeekDay = (count == 1 ? weekDay : (int)usageLog.Date.AddDays(-1).DayOfWeek).ToString() });
			var st = GetStorage(user, usageLog, curfews);

			var c = new CheckTask(st);
			var res = c.Check(user.Login, usageLog.Date);

			Assert.AreEqual(UsageResultKind.Blocked, res.Kind);
		}

		[TestCase]
		public void CheckCurfewWeekDaySeveralMatchUsedMoreThanAllowed()
		{

			var user = GetUser();
			var usageLog = GetUsageLog(user, TimeSpan.FromHours(2));

			var weekDay = (int)usageLog.Date.DayOfWeek;

			var curfews = GetCurfews(3, user, (count, u) => new Curfew { Id = count, User = u, Time = TimeSpan.FromHours(count > 0 ? count : 1), WeekDay = weekDay.ToString() });
			var st = GetStorage(user, usageLog, curfews);

			var c = new CheckTask(st);
			var res = c.Check(user.Login, usageLog.Date);

			Assert.AreEqual(UsageResultKind.Blocked, res.Kind);
		}

		[TestCase]
		public void CheckWeekDayStringSingle()
		{
			var ct = new CheckTask(null);

			CheckWeekDay(ct, "1", new[] { 1 }, new[] { 0, 2, 3, 4, 5, 6, 7 });
		}

		void CheckWeekDay(CheckTask ct, string weekDayString, int[] match, int[] nomatch)
		{
			foreach (var m in match)
				Assert.IsTrue(ct.IsWeekDay(weekDayString, m));
			foreach (var m in nomatch)
				Assert.IsFalse(ct.IsWeekDay(weekDayString, m));
		}

		[TestCase]
		public void CheckWeekDayStringSetOf()
		{
			var ct = new CheckTask(null);
			CheckWeekDay(ct, "1, 3, 5", new[] { 3, 1, 5 }, new[] { 0, 2, 4, 6, 7 });
		}

		[TestCase]
		public void CheckWeekDayStringRange()
		{
			var ct = new CheckTask(null);
			CheckWeekDay(ct, "1 - 5", new[] { 1, 2, 3, 4, 5 }, new[] { 0, 6, 7 });
		}

		[TestCase]
		public void CheckWeekDayStringRangeSetOf()
		{
			var ct = new CheckTask(null);
			CheckWeekDay(ct, "0, 2-4, 6", new[] { 3, 0, 6, 2, 4 }, new[] { 7, 5, 1 });
		}

		[TestCase]
		public void CheckWeekDayEmptyString()
		{
			var ct = new CheckTask(null);
			Assert.Throws<ArgumentException>(() => ct.IsWeekDay(null, 1));
		}

		[TestCase]
		public void CheckWeekDayCharactersString()
		{
			var ct = new CheckTask(null);
			Assert.Throws<ArgumentException>(() => ct.IsWeekDay("asdf", 1));
		}

		[TestCase]
		public void CheckWeekDayMailformedString()
		{
			var ct = new CheckTask(null);
			Assert.Throws<ArgumentException>(() => ct.IsWeekDay("21", 21));
			Assert.Throws<ArgumentException>(() => ct.IsWeekDay("21-24", 22));
		}

		User GetUser() { return new User { Login = "test" }; }
		UsageLog GetUsageLog(User user, TimeSpan used) { return new UsageLog { Date = DateTime.Today, User = user, UserId = user.Id, Used = used }; }

		IEnumerable<Curfew> GetCurfews(int count, User user, Func<int, User, Curfew> curfewInit)
		{
			var res = new Collection<Curfew>();
			while (count-- > 0)
				res.Add(curfewInit(count, user));
			return res;
		}

		Storage GetStorage(User user, UsageLog usageLog, IEnumerable<Curfew> curfews)
		{
			var d = MockRepository.Mock<Data>();
			var st = MockRepository.Mock<Storage>(d);

			st.Expect(_ => _.GetUser(user.Login)).Return(user);
			st.Expect(_ => _.GetUsage(user.Login, usageLog.Date)).Return(usageLog);
			st.Expect(_ => _.GetUserCurfews(user)).Return(curfews);

			return st;
		}
	}
}
