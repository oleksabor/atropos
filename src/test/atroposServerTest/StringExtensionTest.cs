using Atropos.Common.String;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest.Common
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
	}
}
