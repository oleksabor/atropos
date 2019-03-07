using Atropos.Common.Logging;
using Atropos.Common.String;
using Atropos.Server.Factory;
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
	public class Storage : DisposeGently
	{
		IData db;
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public Storage(IData data)
		{
			db = data;
		}

		static Storage()
		{
			LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
		}

		public ITransactionScope BeginTransaction()
		{
			return db.BeginTransaction();
		}

		public UsageLog AddUsage(string login, TimeSpan value, DateTime date, int secondsFinished)
		{
			login = login?.Trim();
			if (login.IsEmpty())
				throw new ArgumentException();

			var user = AddUser(login, $"auto {login}");

			var now = DateTime.Now.TimeOfDay;
			var fiveMinutesBefore = now.Add(TimeSpan.FromSeconds(secondsFinished).Negate());

			var usedBefore = from ul in db.UsageLogs
							 join u in db.Users on ul.UserId equals u.Id
							 where u.Login == login
							 && ul.Date == date
							 orderby ul.Id descending
							 select ul;

			if (usedBefore.Any()) 
			{
				var used = usedBefore.First(); // .Last or .LastOrDefault fails to be converted to the SQL

				if (used.Finished >= fiveMinutesBefore)
				{
					db.UsageLogs.Update(_ => _.Id == used.Id, _ => new UsageLog { Used = _.Used + value, Finished = now });
					return used;
				}
			}
			var started = now.Add(value.Negate());
			var id = db.UsageLogs.InsertWithInt32Identity(() => new UsageLog { Started = started, UserId = user.Id, Date = date, Used = value, Finished = now });
			return db.UsageLogs.First(_ => _.Id == id);
		}

		public virtual IEnumerable<UsageLog> GetUsage(string login, DateTime date)
		{
			var usage = from ul in db.UsageLogs
						join u in db.Users on ul.UserId equals u.Id
						where ul.Date == date 
						&& u.Login == login
						orderby ul.Id
						select ul;
			if (!usage.Any())
				Log.WarnFormat("no usage found for user:{0} on date:{1}", login, date);
			return usage;
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

		public virtual IEnumerable<Curfew> GetUserCurfews(User user)
		{
			return db.Curfews.Where(_ => _.UserId == user.Id).ToList();
		}

		public User AddUser(string login, string name)
		{
			db.Users.InsertOrUpdate(() => new User { Login = login, Name = name }, _ => new User() { Name = name }, () => new User { Login = login });
			return GetUser(login);
		}

		public virtual User GetUser(string login)
		{
			var user = db.Users.LoadWith(_ => _.Curfews).Single(_ => _.Login == login);

			return user;
		}

		public virtual IEnumerable<User> GetUsers()
		{
			return db.Users.ToArray();
		}

		public void RemoveCurfews(string login)
		{
			var user = GetUser(login);
			db.Curfews.Delete(_ => _.UserId == user.Id);
		}

		public void AddCurfews(string login, IEnumerable<Curfew> values)
		{
			var user = GetUser(login);
			foreach (var value in values)
				db.Curfews.InsertWithInt32Identity(() => new Curfew { UserId = user.Id, Break = value.Break, Time = value.Time, WeekDay = value.WeekDay });
		}

		public override void DisposeIt()
		{
			db.Dispose();
		}
	}
}
