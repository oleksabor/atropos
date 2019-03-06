using Atropos.Common.Dto;
using client.Wpf.Data;
using client.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposTest.Client.Gui
{
	[TestFixture]
	public class CurfewGuiTest
	{
		[TestCase]
		public void CopyConstructor()
		{
			var source = new Curfew { Break = TimeSpan.FromMinutes(20), Time = TimeSpan.FromHours(2), WeekDay = "1,2,6,0", Id = 123, };
			var cg = source.ToGui();

			Assert.AreEqual(source.Time, cg.Value.Time);
			Assert.AreEqual(source.Break, cg.Value.Break);
			Assert.AreEqual(source.WeekDay, cg.Value.WeekDay);
			Assert.AreEqual(source.Id, cg.Value.Id);
		}

		[TestCase]
		public void DayOfWeekGetSeveral()
		{
			var source = new Curfew { Break = TimeSpan.FromMinutes(20), Time = TimeSpan.FromHours(2), WeekDay = "1,2,6,0" };
			var cg = source.ToGui();

			Assert.IsTrue(cg.Monday.DaySet);
			Assert.IsTrue(cg.Tuesday.DaySet);
			Assert.IsTrue(cg.Saturday.DaySet);
			Assert.IsTrue(cg.Sunday.DaySet);

			Assert.IsFalse(cg.Wednesday.DaySet);
			Assert.IsFalse(cg.Thursday.DaySet);
			Assert.IsFalse(cg.Friday.DaySet);
		}

		[TestCase]
		public void DayOfWeekGetNone()
		{
			var source = new Curfew { Break = TimeSpan.FromMinutes(20), Time = TimeSpan.FromHours(2) };
			var cg = source.ToGui();

			Assert.IsFalse(cg.Monday.DaySet);
			Assert.IsFalse(cg.Tuesday.DaySet);
			Assert.IsFalse(cg.Saturday.DaySet);
			Assert.IsFalse(cg.Sunday.DaySet);

			Assert.IsFalse(cg.Wednesday.DaySet);
			Assert.IsFalse(cg.Thursday.DaySet);
			Assert.IsFalse(cg.Friday.DaySet);
		}
	}
}
