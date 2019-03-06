using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atropos.Common
{
	public class Settings
	{
		public Connection Connection { get; set; }
	}

	public class Connection
	{
		public int Port { get; set; }
	}
}
