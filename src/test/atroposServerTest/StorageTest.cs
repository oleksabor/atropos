using Atropos.Server.Db;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace atroposServerTest
{
	[TestFixture]
	public class StorageTest
	{
		Regex nameSplit = new Regex(@"(.*)(\\)(.*)"); 

		[TestCase]
		public void AddUser()
		{
			var login = WindowsIdentity.GetCurrent().Name;
			var m = nameSplit.Match(login);
			foreach (Group mg in m.Groups)
				login = mg.Value;

			using (var s = new Storage(new Data()))
			{
				var u = s.AddUser(login, "test user");

				Assert.AreEqual(login, u.Login);

				var u2 = s.GetUser(login);

				Assert.AreEqual(u.Id, u2.Id);

				var u3 = s.AddUser(login, "test user another");

				Assert.AreEqual(u.Login, u3.Login);
				Assert.AreNotEqual(u.Name, u3.Name);
			}
		}
	}
}
