using Atropos.Server.Db;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest
{
	public partial class TestDB : LinqToDB.Data.DataConnection
	{
		public ITable<User> Users { get { return this.GetTable<User>(); } }
		public ITable<Curfew> Curfews { get { return this.GetTable<Curfew>(); } }
		public ITable<UsageLog> UsageLogs { get { return this.GetTable<UsageLog>(); } }

		public TestDB()
		{
			SetDt();
		}

		public TestDB(string configuration)
			: base(configuration)
		{
			SetDt();
		}

		void SetDt()
		{
			AddMappingSchema(new TestMappingSchema());
		}

	}

	class TestMappingSchema : MappingSchema
	{
		public TestMappingSchema()
		{
			SetDataType(typeof(TimeSpan), DataType.Int64);

			SetConvertExpression<TimeSpan, DataParameter>(x => DataParameter.Int64(null, (long)x.TotalSeconds));

			SetConvertExpression<long, TimeSpan>(x => x > 0 ? TimeSpan.FromSeconds(x) : TimeSpan.Zero, true);
		}
	}
}
