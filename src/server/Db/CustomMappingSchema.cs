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
