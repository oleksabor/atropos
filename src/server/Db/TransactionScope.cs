using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Db
{
	public interface ITransactionScope : IDisposable
	{
		void Commit();
		void Rollback();
	}

	internal class TransactionScope : ITransactionScope
	{
		DataConnectionTransaction _value;

		internal TransactionScope(DataConnectionTransaction value)
		{
			_value = value;
		}

		public void Commit()
		{
			_value.Commit();
		}

		public void Rollback()
		{
			_value.Rollback();
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_value.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}
