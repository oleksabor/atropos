using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting.Test
{
	[TestFixture]
	public class RemoteAccessTest
	{
		[TestCase]
		public void PerformActionExceptionDisconnect()
		{
			var remoteService = MockRepository.Mock<IService>();
			remoteService.Stub(_ => _.MethodException()).Throws<ArgumentException>();
			var disconnected = false;

			var ra = new RemoteAccess<IService>(() => remoteService, () => disconnected = true);

			Assert.Throws<ArgumentException>(() => ra.Perform(_ => _.MethodException()));

			Assert.IsTrue(disconnected);
		}

		[TestCase]
		public void PerformAction()
		{
			var remoteService = MockRepository.Mock<IService>();
			remoteService.Stub(_ => _.Method()).Repeat.Once();
			var disconnected = false;

			var ra = new RemoteAccess<IService>(() => remoteService, () => disconnected = true);

			ra.Perform(_ => _.Method());

			Assert.IsFalse(disconnected);

			remoteService.VerifyAllExpectations();
		}

		[TestCase]
		public void PerformFunction()
		{
			var remoteService = MockRepository.Mock<IService>();
			remoteService.Stub(_ => _.Method2()).Repeat.Once().Return("from remote service");
			var disconnected = false;

			var ra = new RemoteAccess<IService>(() => remoteService, () => disconnected = true);

			var result = ra.Perform(_ => _.Method2());

			Assert.IsFalse(disconnected);
			Assert.AreEqual("from remote service", result);
			remoteService.VerifyAllExpectations();
		}

		[TestCase]
		public void CheckIsRemoteReady()
		{
			var remoteService = MockRepository.Mock<IService>();
			var disconnected = 0;

			var attempts = 0;

			var ra = new RemoteAccess<IService>(() => remoteService, () => ++disconnected);
			ra.CheckIsRemoteReady(_ => { if (++attempts < 5) throw new ApplicationException("not ready yet"); });

			Assert.AreEqual(5, attempts);
			Assert.AreEqual(4, disconnected);
		}

		[TestCase]
		public void CheckIsRemoteNotReady()
		{
			var remoteService = MockRepository.Mock<IService>();
			var disconnected = 0;

			var ra = new RemoteAccess<IService>(() => remoteService, () => ++disconnected);
			Assert.Throws<ApplicationException>(() => ra.CheckIsRemoteReady(_ => throw new ApplicationException("not ready yet"), 1));

			Assert.AreEqual(1, disconnected);
		}

	}

	public interface IService
	{
		void Method();

		string Method2();

		void MethodException();
	}
}
