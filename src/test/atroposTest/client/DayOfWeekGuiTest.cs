using Atropos.Common.Dto;
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
	public class DayOfWeekGuiTest
	{
		[TestCase]
		public void DaySetEmptyDto()
		{
			var dto = new Curfew { };

			var dwg = new DayOfWeekGui(DayOfWeek.Monday, dto);

			dwg.DaySet = false;
			Assert.IsTrue(string.IsNullOrEmpty(dto.WeekDay));

			dwg.DaySet = true;
			Assert.AreEqual("1", dto.WeekDay);
		}

		[TestCase]
		public void DaySetAndRemoveSingle()
		{
			var dto = new Curfew { };

			var dwg = new DayOfWeekGui(DayOfWeek.Monday, dto);

			dwg.DaySet = true;
			Assert.AreEqual("1", dto.WeekDay);

			dwg.DaySet = false;
			Assert.AreEqual("", dto.WeekDay);
		}

		[TestCase]
		public void DaySetAndRemoveTwo()
		{
			var dto = new Curfew { WeekDay="6" };

			var dwg = new DayOfWeekGui(DayOfWeek.Monday, dto);

			dwg.DaySet = true;
			Assert.AreEqual("6,1", dto.WeekDay);

			dwg.DaySet = false;
			Assert.AreEqual("6", dto.WeekDay);
		}

		[TestCase]
		public void DaySetAndRemoveSeveral()
		{
			var dto = new Curfew { WeekDay = "6,5" };

			var dwg = new DayOfWeekGui(DayOfWeek.Monday, dto);

			dwg.DaySet = true;
			Assert.AreEqual("6,5,1", dto.WeekDay);

			dwg.DaySet = false;
			Assert.AreEqual("6,5", dto.WeekDay);

			var dwgTh = new DayOfWeekGui(DayOfWeek.Thursday, dto);
			dwgTh.DaySet = true;
			Assert.AreEqual("6,5,4", dto.WeekDay);

			dwg.DaySet = true;
			Assert.AreEqual("6,5,4,1", dto.WeekDay);

			dwgTh.DaySet = false;
			Assert.AreEqual("6,5,1", dto.WeekDay);
		}
	}
}
