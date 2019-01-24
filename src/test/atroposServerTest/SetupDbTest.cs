using Atropos.Server.Logging;
using LinqToDB;
using LinqToDB.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atroposServerTest
{
	[SetUpFixture]
	public class SetupDbTest
	{
		static ILog Log = LogProvider.GetLogger("dbTest");

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Environment.CurrentDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..");
			// or identically under the hoods
			//Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
			LinqToDB.Data.DataConnection.TurnTraceSwitchOn();
			LinqToDB.Data.DataConnection.WriteTraceLine = (message, displayName) => { Log.Debug($"{message} {displayName}"); };
			//LinqToDB.Mapping.MappingSchema.SetDataType(typeof(TimeSpan), DataType.NText);

			//var mapping = LinqToDB.Mapping.MappingSchema.Default;
			//mapping.SetDataType(typeof(TimeSpan), DataType.Int64);
			//mapping.AddScalarType(typeof(TimeSpan), DataType.Time);

			//mapping.SetConvertExpression<TimeSpan, DataParameter>(x => DataParameter.Time(null, x));

			//// There is no special handling for nullable types, so we should register it explicitly.
			//mapping.SetConvertExpression<TimeSpan?, DataParameter>(x => new DataParameter(null, x.HasValue ? (object)x.Value : null, DataType.Time));

			//// Convert from underlying type to our type
			//// If type contains appropriate consturction - we can did not define this mapping 

			//mapping.SetConvertExpression(DbDataType. x => new TimeSpan(x));
		}

	}
}
