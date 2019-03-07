using Atropos.Server.Factory;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace atroposTest.Factory
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

		[TestCase]
		public void RunExceptionHandler()
		{
			var runCount = 0;

			int errCount = 0;

			var rt = MockRepository.Mock<BackgroundTask>();
			rt.Stub(_ => _.Run()).WhenCalled(() => { runCount++; throw new NotImplementedException(); });

			rt.HandleFault = (e, c) => errCount = c;

			rt.Start(1);
			Thread.Sleep(3000);
			rt.Stop();

			Assert.IsTrue(runCount >= 3);

			Assert.IsTrue(errCount >= 3);
		}

		[TestCase]
		public void StartMoreThanOnce()
		{
			int runCount = 0;

			var rt = MockRepository.Mock<BackgroundTask>();
			rt.Stub(_ => _.Run()).WhenCalled(() => runCount++);

			rt.Start(20);
			rt.Start(1);
			Thread.Sleep(5000);
			rt.Stop();

			Assert.AreEqual(1, runCount);

			rt.Start(20);
			rt.Start(1);
			Thread.Sleep(5000);
			rt.Stop();

			Assert.AreEqual(2, runCount);
		}

		[TestCase]
		public void RunOnStop()
		{
			int runCount = 0;

			var rt = MockRepository.Mock<RunOnStopTest>(true);
			rt.Stub(_ => _.Run()).WhenCalled(() => runCount++);

			rt.Stop();

			Assert.AreEqual(1, runCount);

			rt = MockRepository.Mock<RunOnStopTest>(false);
			rt.Stub(_ => _.Run()).WhenCalled(() => runCount++);

			rt.Stop();

			Assert.AreEqual(1, runCount);
		}

		public abstract class RunOnStopTest : BackgroundTask
		{
			public RunOnStopTest(bool value)
			{
				RunOnStop = value;
			}
		}

	}
}
