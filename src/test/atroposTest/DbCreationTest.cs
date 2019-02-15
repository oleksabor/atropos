using Atropos.Common.Logging;
using Atropos.Server.Db;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest.Db
{
	/// <summary>
	/// this test creates db 
	/// </summary>
	[TestFixture]
	public class DbCreationTest
    {
		static ILog Log = LogProvider.GetCurrentClassLogger();
		
		[TestCase]
		public void CreateDatabase()
		{
			var fileName = Path.Combine(Environment.CurrentDirectory, @"TestData.sqlite");
			if (File.Exists(fileName))
			{
				Log.Info($"removing {fileName}");
				File.Delete(fileName);
			}
				//LinqToDB.DataProvider.SQLite.SQLiteTools.CreateDatabase(fileName);

			using (var db = new Data())
				try
				{
					var c = db.CreateCommand();

					CreateTableIfNotExists<User>(db);
					CreateTableIfNotExists<Curfew>(db);
					CreateTableIfNotExists<UsageLog>(db);
				}
				catch (Exception e)
				{
					throw new ApplicationException($"failed to create database with current dir {fileName}", e);
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

		Random _rnd = new Random();

		[TestCase]
		public void InsertData()
		{

			using (var db = new Data())
				try
				{
					var u = new User() { Login = "testLogin", Name = "testName" };
					var i = db.InsertWithInt32Identity(u);

					u.Id = i;

					var totalTime = new TimeSpan(GetHour(), GetMinute(), 00);
					var cf = new Curfew() { UserId = i, WeekDay = "Mo", Time = totalTime };
					var cfi = db.InsertWithInt32Identity(cf);

					var user = db.Users.LoadWith(_ => _.Curfews).FirstOrDefault(_ => _.Id == i);
					Assert.IsNotNull(user);

					var curfew = db.Curfews.LoadWith(_ => _.User).FirstOrDefault(_ => _.Id == cfi);
					Assert.IsNotNull(curfew);
					Assert.AreEqual(TimeSpan.Zero, curfew.Break);
					Assert.AreEqual(totalTime, curfew.Time);

					Assert.AreEqual(1, user.Curfews.Count());
					Assert.IsNotNull(curfew.User);

					var testTime = new TimeSpan(GetHour(), GetMinute(), 00);
					var testBreakTime = new TimeSpan(0, _rnd.Next(20, 59), 0);
					var cfBreak = new Curfew() { UserId = i, WeekDay = "Mo", Time = testTime, Break = testBreakTime };
					var cfiBreak = db.InsertWithInt32Identity(cfBreak);
					cfBreak = db.Curfews.LoadWith(_ => _.User).FirstOrDefault(_ => _.Id == cfiBreak);

					Assert.AreEqual(i, cfBreak.UserId);
					Assert.AreEqual(testTime, cfBreak.Time);
					Assert.AreEqual(testBreakTime, cfBreak.Break);

					var usageDate = DateTime.Today;
					var usedTime = new TimeSpan(GetHour(), GetMinute(), 0);
					var startTime = new TimeSpan(GetHour(), GetMinute(), GetMinute());

					var usage = new UsageLog() { UserId = user.Id, Date = usageDate, Used = usedTime, Started = startTime };
					var usageId = db.InsertWithInt32Identity(usage);
					usage = db.UsageLogs.FirstOrDefault(_ => _.Id == usageId);

					Assert.AreEqual(user.Id, usage.UserId);
					Assert.AreEqual(usedTime, usage.Used);
					Assert.AreEqual(usageDate, usage.Date);
					Assert.AreEqual(startTime, usage.Started);

				}
				catch (LinqToDBConvertException l2dbe)
				{
					Console.WriteLine("{0}", l2dbe);
					throw;
				}
		}

		int GetHour()
		{
			return _rnd.Next(0, 23);
		}

		int GetMinute()
		{
			return _rnd.Next(0, 59);
		}

		//private const string ConnectionString = "Data Source=:memory:;";

		//[TestCase]
		//public void SQliteDataProviderTimeSpan()
		//{
		//	var dataProvider = new SQLiteDataProvider();

		//	var connection = dataProvider.CreateConnection(ConnectionString);
		//	connection.Open();

		//	var dataConnection = new DataConnection(dataProvider, connection);

		//	dataConnection.MappingSchema.SetDataType(typeof(TimeSpan), DataType.Int64);

		//	dataConnection.CreateTable<Category>();

		//	dataConnection.GetTable<Category>()
		//		.DataContext
		//		.Insert(new Category
		//		{
		//			Id = 2,
		//			Time = new TimeSpan(10, 0, 0)
		//		});


		//	foreach (var category in dataConnection.GetTable<Category>())
		//	{
		//		Console.WriteLine($@"Id: {category.Id}, Time: {category.Time}");
		//	}
		//}

		//private class Category
		//{
		//	public int Id { get; set; }
		//	public TimeSpan Time { get; set; }
		//}
	}
}
