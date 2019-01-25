using LinqToDB;
using LinqToDB.Data;
using System;

namespace Atropos.Server.Db
{
	public interface IData : IDisposable
	{
		ITable<Curfew> Curfews { get; }
		ITable<UsageLog> UsageLogs { get; }
		ITable<User> Users { get; }

		/// <summary>
		/// Begins the transaction for current DAL instance.
		/// </summary>
		/// <returns></returns>
		ITransactionScope BeginTransaction();
	}
}