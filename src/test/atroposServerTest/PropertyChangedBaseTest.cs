using client.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposTest.Client.Data
{
	[TestFixture]
	public class PropertyChangedBaseTest : PropertyChangedBase
	{
		[TestCase]
		public void SetObject()
		{
			var o = new object();
			One = o;

			Assert.AreSame(o, One);
		}

		[TestCase]
		public void SetInt()
		{
			var o = 12;
			Two = o;

			Assert.AreEqual(o, Two);
		}

		private object _one;
		public object One { get { return _one; } set { Set(ref _one, value); } }

		private int _two;
		public int Two { get { return _two; } set { Set(ref _two, value); } }
	}
}
