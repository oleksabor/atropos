using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest
{
	[SetUpFixture]
	public class MySetUpClass
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
			// or identically under the hoods
			//Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
		}

	}
}
