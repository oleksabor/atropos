using Atropos.Common;
using Atropos.Server.Factory;
using com.Tools.WcfHosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Listener
{
	public class DataServiceHost : DisposeGently
	{
		public DataServiceHost(DataService value, IWcfHost host, CommunicationSettings config) 
		{
			Value = value;
			Host = host;
			Config = config;
			Host.AddHost(Value, Config);
		}

		public IDataService Value { get; }
		public IWcfHost Host { get; }
		public CommunicationSettings Config { get; }

		public override void DisposeIt()
		{
			Host.CloseHost();
		}
	}
}
