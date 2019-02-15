﻿using Atropos.Common.Dto;
using client.Wpf.Data;
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
			var source = new Curfew { Break = TimeSpan.FromMinutes(20), Time = TimeSpan.FromHours(2), WeekDay = "1,2,6,0" };
			var cg = new CurfewGui(source);

			Assert.AreEqual(source.Time, cg.Time);
			Assert.AreEqual(source.Break, cg.Break);
			Assert.AreEqual(source.WeekDay, cg.WeekDay);
		}

		[TestCase]
		public void DayOfWeekGetSeveral()
		{
			var source = new Curfew { Break = TimeSpan.FromMinutes(20), Time = TimeSpan.FromHours(2), WeekDay = "1,2,6,0" };
			var cg = new CurfewGui(source);

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
			var cg = new CurfewGui(source);

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