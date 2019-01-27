using Atropos.Common.Logging;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Atropos.Server.Db
{
	public class StorageTool
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		Data db;

		public StorageTool(Data data)
		{
			db = data;
		}

		Regex dbNameRegex = new Regex(@"Data\s+Source=(?<n>.+)");

		public string GetDbName(string connectionString)
		{
			var parts = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var part in parts)
			{
				var dbname = dbNameRegex.Match(part);
				if (dbname.Success)
				return dbname.Groups["n"].Value;
			}
			throw new ArgumentException(string.Format("can't get dbName from connection string", connectionString));
		}

		public void CheckDb()
		{
			Log.Debug("checking");
			var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetDbName(db.ConnectionString));

			if (!File.Exists(filename))
				CreateDatabase(filename);
		}

		public void CreateDatabase(string fileName)
		{
			Log.Debug($"creating {fileName}");
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
			Log.Debug("created");
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
	}
}
