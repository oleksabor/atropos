using Atropos.Server.Db;
using LinqToDB;
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

		public TestDB()
		{
		}

		public TestDB(string configuration)
			: base(configuration)
		{
		}
	}

}
