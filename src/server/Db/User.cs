using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Db
{
	[DebuggerDisplay("{Id} {Login} {Name}")]
	public class User
	{
		[PrimaryKey, Identity]	public int Id { get; set; }

		[Column(CreateFormat = "{0} {1} {2} {3} COLLATE NOCASE")]
		public string Login { get; set; }

		[Column]				public string Name { get; set; }

		[Association(ThisKey = "Id", OtherKey = "UserId")]
		public List<Curfew> Curfews { get; set; }
	}
}
