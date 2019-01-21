using Atropos.Server.Db;
using LinqToDB;
using LinqToDB.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest
{
	[TestFixture]
	public class DbCreationTest
    {
		[TestCase]
		public void CreateDatabase()
		{
			var path = Path.GetDirectoryName(Environment.CurrentDirectory);
			var fileName = Path.Combine(path, @"TestData.sqlite");
			if (!File.Exists(fileName))
				LinqToDB.DataProvider.SQLite.SQLiteTools.CreateDatabase(fileName);

			using (var db = new TestDB())
				try
				{
					var c = db.CreateCommand();

					CreateTableIfNotExists<User>(db);
					CreateTableIfNotExists<Curfew>(db);
				}
				catch (Exception e)
				{
					throw new ApplicationException(string.Format("failed to create database with current dir {0}", path), e);
				}

		}

		void CreateTableIfNotExists<TDto>(DataConnection conn)
		{
			var sp = conn.DataProvider.GetSchemaProvider();
			var dbSchema = sp.GetSchema(conn);
			var tableName = typeof(TDto).Name;
			if (!dbSchema.Tables.Any(t => t.TypeName == tableName))
			{
				//no required table-create it
				conn.CreateTable<TDto>();
			}
		}

		[TestCase]
		public void InsertData()
		{
			using (var db = new TestDB())
			{
				var u = new User() { Login = "testLogin", Name = "testName" };
				var i = db.InsertWithInt32Identity(u);

				u.Id = i;

				var cf = new Curfew() { UserId = i, WeekDay = "Mo" };
				var cfi = db.InsertWithInt32Identity(cf);

				var user = db.Users.LoadWith(_ => _.Curfews).FirstOrDefault(_ => _.Id == i);
				Assert.IsNotNull(user);
				var curfew = db.Curfews.LoadWith(_ => _.User).FirstOrDefault(_ => _.Id == cfi);
				Assert.IsNotNull(curfew);

				Assert.AreEqual(1, user.Curfews.Count());
				Assert.IsNotNull(curfew.User);
			}
		}
	}
}
