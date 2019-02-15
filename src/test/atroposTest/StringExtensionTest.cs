using Atropos.Common.String;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposTest.Common
{
	[TestFixture]
	public class StringExtensionTest
	{
		[TestCase]
		public void TestDayOfWeekAsString()
		{
			Assert.AreEqual("Mo", StringExtension.AsString(DayOfWeek.Monday));
			Assert.AreEqual("Th", StringExtension.AsString(DayOfWeek.Thursday));
			Assert.AreEqual("Su", StringExtension.AsString(DayOfWeek.Sunday));
		}

		[TestCase]
		public void CheckWeekDayStringSingle()
		{
			CheckWeekDay("1", new[] { 1 }, new[] { 0, 2, 3, 4, 5, 6 });
		}

		void CheckWeekDay(string weekDayString, int[] match, int[] nomatch)
		{
			foreach (var m in match)
				Assert.IsTrue(StringExtension.IsWeekDay(weekDayString, m));
			foreach (var m in nomatch)
				Assert.IsFalse(StringExtension.IsWeekDay(weekDayString, m));
		}

		[TestCase]
		public void CheckWeekDayStringSetOf()
		{
			CheckWeekDay("1, 3, 5", new[] { 3, 1, 5 }, new[] { 0, 2, 4, 6 });
		}

		[TestCase]
		public void CheckWeekDayStringRange()
		{
			CheckWeekDay("1 - 5", new[] { 1, 2, 3, 4, 5 }, new[] { 0, 6 });
		}

		[TestCase]
		public void CheckWeekDayStringRangeSetOf()
		{ 
			CheckWeekDay("0, 2-4, 6", new[] { 3, 0, 6, 2, 4 }, new[] { 5, 1 });
		}

		[TestCase]
		public void CheckWeekDayEmptyString()
		{
			Assert.Throws<ArgumentException>(() => StringExtension.IsWeekDay(null, 1));
		}

		[TestCase]
		public void CheckWeekDayCharactersString()
		{
			Assert.Throws<ArgumentException>(() => StringExtension.IsWeekDay("asdf", 1));
		}

		[TestCase]
		public void CheckWeekDayMailformedString()
		{
			Assert.Throws<ArgumentException>(() => StringExtension.IsWeekDay("21", 21));
			Assert.Throws<ArgumentException>(() => StringExtension.IsWeekDay("21-24", 22));
		}
	}
}
