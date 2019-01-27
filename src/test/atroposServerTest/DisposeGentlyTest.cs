using Atropos.Server.Factory;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest
{
	[TestFixture]
	public class DisposeGentlyTest
	{
		[TestCase]
		public void Dispose()
		{
			DisposeGently dt;
			using (dt = MockRepository.Mock<DisposeGently>())
			{
				dt.Stub(_ => _.DisposeIt()).Repeat.Once();
			}

			dt.VerifyAllExpectations();
		}

	}
}
