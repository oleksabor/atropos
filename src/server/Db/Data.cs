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
	public partial class Data : LinqToDB.Data.DataConnection
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

	}

	class CustomMappingSchema : MappingSchema
	{
		public CustomMappingSchema()
		{
			SetDataType(typeof(TimeSpan), DataType.Int64);

			SetConvertExpression<TimeSpan, DataParameter>(x => DataParameter.Int64(null, (long)x.TotalSeconds));

			SetConvertExpression<long, TimeSpan>(x => x > 0 ? TimeSpan.FromSeconds(x) : TimeSpan.Zero, true);
		}
	}
}
