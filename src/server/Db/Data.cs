using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Db
{
	public partial class Data : LinqToDB.Data.DataConnection, IData
	{
		public ITable<User> Users { get { return this.GetTable<User>(); } }
		public ITable<Curfew> Curfews { get { return this.GetTable<Curfew>(); } }
		public ITable<UsageLog> UsageLogs { get { return this.GetTable<UsageLog>(); } }

		public Data()
		{
			SetDt();
		}

		public Data(string configuration)
			: base(configuration)
		{
			SetDt();
		}

		void SetDt()
		{
			AddMappingSchema(new CustomMappingSchema());
		}

		ITransactionScope IData.BeginTransaction()
		{
			return new TransactionScope(base.BeginTransaction());
		}
	}
}
