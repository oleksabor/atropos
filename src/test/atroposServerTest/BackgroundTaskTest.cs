using Atropos.Server.Factory;
using NUnit.Framework;
using Rhino.Mocks;
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
			var runCount = 0;

			var rt = MockRepository.Mock<BackgroundTask>();
			rt.Stub(_ => _.Run()).WhenCalled(() => runCount++);

			rt.Start(1);
			Thread.Sleep(3000);
			rt.Stop();

			Assert.IsTrue(runCount >= 3);
		}

		[TestCase]
		public void RunException()
		{
			var runCount = 0;

			var rt = MockRepository.Mock<BackgroundTask>();
			rt.Stub(_ => _.Run()).WhenCalled(() => { runCount++; throw new NotImplementedException(); });

			rt.Start(1);
			Thread.Sleep(3000);
			rt.Stop();

			Assert.IsTrue(runCount >= 3);
		}
	}
}
