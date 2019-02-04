using Atropos.Server.Db;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest.Db
{
	[TestFixture]
	public class StorageToolTest
	{
		[TestCase]
		public void GetDbName()
		{
			var st = new StorageTool(null);

			var dbname = "TestData.sqlite";

			var name = st.GetDbName($"Data Source={dbname}");
			Assert.AreEqual("TestData.sqlite", name);

			name = st.GetDbName($"Data Source={dbname};");
			Assert.AreEqual("TestData.sqlite", name);

			name = st.GetDbName($"Data Source={dbname};Version=3");
			Assert.AreEqual("TestData.sqlite", name);
		}

		[TestCase]
		public void GetDbNameError()
		{
			var st = new StorageTool(null);
			Assert.Throws<ArgumentException>(() => st.GetDbName("Version=3;New=true"));
		}
	}
}
