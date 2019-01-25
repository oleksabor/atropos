using LinqToDB;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Db
{
	[DebuggerDisplay("{Id} {UserId} {WeekDay} {Time}")]
	public class Curfew
	{
		[PrimaryKey, Identity]	public int Id { get; set; }
		[Column]				public string WeekDay { get; set; }
		[Column]				public TimeSpan Time { get; set; }
		[Column, Nullable]		public TimeSpan Break { get; set; }
		[Column]				public int UserId { get; set; }

		#region Associations

		/// <summary>
		/// FK_User_BackReference
		/// </summary>
		[Association(ThisKey = "UserId", OtherKey = "Id", CanBeNull = false)]
		public User User { get; set; }

		#endregion

	}
}
