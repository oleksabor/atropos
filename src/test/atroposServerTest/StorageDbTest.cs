using Atropos.Server.Db;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace atroposServerTest.Db
{
	/// <summary>
	/// this test requires actual db to be created
	/// </summary>
	[TestFixture]
	public class StorageDbTest
	{
		Regex nameSplit = new Regex(@"(.*)(\\)(.*)");

		string Login
		{
			get
			{
				var login = WindowsIdentity.GetCurrent().Name;
				var m = nameSplit.Match(login);
				foreach (Group mg in m.Groups)
					login = mg.Value;
				return login;
			}
		}

		[TestCase]
		public void AddUser()
		{

			using (var s = new Storage(new Data()))
			{
				var u = s.AddUser(Login, "test user");

				Assert.AreEqual(Login, u.Login);

				var u2 = s.GetUser(Login);

				Assert.AreEqual(u.Id, u2.Id);

				var u3 = s.AddUser(Login, "test user another");

				Assert.AreEqual(u.Login, u3.Login);
				Assert.AreNotEqual(u.Name, u3.Name);
			}
		}

		[TestCase]
		public void AddCurfew()
		{
			var login = string.Format("{0}-{1:mmss}", Login, DateTime.Now);
			var ws = DateTime.Today.DayOfWeek.ToString();
			Curfew c;
			using (var s = new Storage(new Data()))
			{
				using (var t = s.BeginTransaction())
				{
					c = s.AddCurfew(login, TimeSpan.FromHours(3), TimeSpan.FromMinutes(20), ws);

					t.Commit();
				}
				var u2 = s.GetUser(login);

				Assert.IsTrue(1 <= u2.Curfews.Count);

				Assert.AreEqual(c.UserId, u2.Id);
			}
		}

		[TestCase]
		public void AddCurfewAndRollback()
		{
			Curfew c;
			var ws = DateTime.Today.DayOfWeek.ToString();
			using (var s = new Storage(new Data()))
			{
				using (var t = s.BeginTransaction())
				{
					c = s.AddCurfew(Login, TimeSpan.FromHours(3), TimeSpan.FromMinutes(20), ws);

					t.Rollback();
				}

				var cs = s.GetUserCurfews(s.GetUser(Login));
				Assert.IsFalse(cs.Any(_ => _.Id == c.Id));
			}
		}

		[TestCase]
		public void UsageLogAdd()
		{
			UsageLog usage = null;
			var n = DateTime.Now;
			var ts = TimeSpan.FromSeconds(n.TimeOfDay.Minutes * n.TimeOfDay.Seconds);

			using (var s = new Storage(new Data()))
			{
				using (var t = s.BeginTransaction())
				{
					usage = s.AddUsage(Login, ts, n.Date);

					t.Commit();
				}

				Assert.AreEqual(ts, usage.Used);
				Assert.AreEqual(n.Date, usage.Date);

				Assert.AreEqual(s.GetUser(Login).Id, usage.UserId);
			}
		}

	}

}
