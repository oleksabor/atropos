using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common
{
	public enum UsageResultKind
	{
		Unknown, 
		NoRestriction,
		Break,
		Blocked,
	}

	public class UsageResult
	{
		public UsageResultKind Kind { get; set; }

		public TimeSpan Used { get; set; }
	}
}
