using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Db
{
	public partial class UsageLog
	{
		public UsageLog()
		{ }

		public UsageLog(TimeSpan started, TimeSpan used)
		{
			Started = started;
			Used = used;
			Finished = started + used;
		}
	}
}
