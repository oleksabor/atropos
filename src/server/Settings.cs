using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server
{
	public class Settings
	{
		public Intervals Interval { get; set; }

		public Settings()
		{
			Interval = new Intervals();
		}
	}

	public class Intervals
	{
		public int Woodpecker => 15;
		public int Accounter => 10;
		public int Locker => 30;
	}

	public class ServiceOptions
	{
		public string Name { get; set; }
	}
}
