using Atropos.Server.Factory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace atroposServerTest
{
	[TestFixture]
	public class BackgroundTaskTest
	{
		[TestCase]
		public void Run()
		{
			var rt = new RunTest();
			rt.Start(1);
			Thread.Sleep(3000);

			rt.Stop();

			Assert.IsTrue(rt.RunCount >= 3);
		}

		public class RunTest : BackgroundTask
		{
			public override void Run()
			{
				RunCount++;
			}

			public int RunCount;
		}
	}
}
