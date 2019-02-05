using Atropos.Common.Logging;
using LinqToDB;
using LinqToDB.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest.Db
{
	[SetUpFixture]
	public class SetupDbTest
	{
		static ILog Log = LogProvider.GetLogger("dbTest");

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Environment.CurrentDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..");
			// or identically under the hoods
			//Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);


			LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
#if DEBUG
			LinqToDB.Data.DataConnection.TurnTraceSwitchOn();
			LinqToDB.Data.DataConnection.WriteTraceLine = (message, displayName) => { Log.Debug($"{message} {displayName}"); };
#endif
		}

	}
}
