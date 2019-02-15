using Atropos.Server.Db;
using Atropos.Server.Factory;
using Atropos.Server.Listener;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposTest.Listener
{
	[TestFixture]
	public class DataServiceTest
	{
		[TestCase]
		public void Users()
		{
			var data = MockRepository.Mock<IData>();
			var st = MockRepository.Mock<Storage>(data);
			st.Expect(_ => _.GetUsers()).Returns(() => new[] { new User { Id = 1, Login = "1login", Name = "1 name" } }).Repeat.Once();


			var ds = new DataService(st);

			var users = ds.GetUsers();

			Assert.IsNotNull(users);

			Assert.AreEqual(1, users.Length);
			var dto = users[0];

			Assert.AreEqual(1, dto.Id);

			st.VerifyAllExpectations();
		}
	}
}
