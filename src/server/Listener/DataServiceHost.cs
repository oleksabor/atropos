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
	public class DataServiceHost<T> : DisposeGently
	{
		public DataServiceHost(IWcfHost host, CommunicationSettings config, StructureMapServiceBehavior smap) 
		{
			Host = host;
			Config = config;
			Host.AddHostType<T>(Config, new[] { smap });
		}

		public IWcfHost Host { get; }
		public CommunicationSettings Config { get; }

		public override void DisposeIt()
		{
			Host.CloseHost();
		}
	}
}
