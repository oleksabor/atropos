using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Event
{
	public enum Kind
	{
		Connected,
		Disconnected,
		Unknown,
		Locked,
		Active,
	}
}
