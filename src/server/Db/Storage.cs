using Atropos.Common.String;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Db
{
	/// <summary>
	/// DAL logic implementation
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public class Storage : IDisposable
	{
		IData db;

		public Storage(IData data)
		{
			db = data;
		}

		public ITransactionScope BeginTransaction()
		{
			return db.BeginTransaction();
		}

		public UsageLog AddUsage(string login, TimeSpan value, DateTime date)
		{
			login = login?.Trim();
			if (login.IsEmpty())
				throw new ArgumentException();

			var user = AddUser(login, $"auto {login}");

			db.UsageLogs.InsertOrUpdate(() => new UsageLog { Started = DateTime.Now.TimeOfDay, UserId = user.Id, Date = date, Used = value }, 
				_ => new UsageLog { Used = _.Used + value }, 
				() => new UsageLog { Date = date, UserId = user.Id });

			var usage = db.UsageLogs.Single(_ => _.Date == date && _.UserId == user.Id);
			return usage;
		}

		public User AddUser(string login, string name)
		{
			db.Users.InsertOrUpdate(() => new User { Login = login, Name = name }, _ => new User() { Name = name }, () => new User { Login = login });
			return GetUser(login);
		}

		public Curfew AddCurfew(string login, TimeSpan time, TimeSpan breakTime, string weekDay)
		{
			var user = AddUser(login, $"auto {login}");

			var id = db.Curfews.InsertWithInt32Identity(() => new Curfew { UserId = user.Id, Time = time, Break = breakTime, WeekDay = weekDay });

			return db.Curfews.Single(_ => _.Id == id);
		}

		public Curfew UpdateCurfew(Curfew value)
		{
			db.Curfews.Update(_ => new Curfew { Id = value.Id }, _ => new Curfew { Time = value.Time, Break = value.Break, WeekDay = value.WeekDay });

			return db.Curfews.Single(_ => _.Id == value.Id);
		}

		public User GetUser(string login)
		{
			var user = db.Users.LoadWith(_ => _.Curfews).Single(_ => _.Login == login);

			return user;
		}

		public IEnumerable<Curfew> GetUserCurfews(User user)
		{
			return db.Curfews.Where(_ => _.UserId == user.Id);
		}

		public void Dispose()
		{
			if (db != null)
			{
				db.Dispose();
				db = null;
			}
		}
	}
}
