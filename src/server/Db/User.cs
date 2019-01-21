﻿using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Db
{
	public class User
	{
		[PrimaryKey, Identity]	public int Id { get; set; }
		[Column]				public string Login { get; set; }
		[Column]				public string Name { get; set; }

		[Association(ThisKey = "Id", OtherKey = "UserId")]
		public List<Curfew> Curfews { get; set; }
	}
}