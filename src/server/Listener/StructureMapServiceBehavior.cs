using Atropos.Server.Factory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Listener
{
	//https://lostechies.com/jimmybogard/2008/07/30/integrating-structuremap-with-wcf/
	public class StructureMapServiceBehavior : IServiceBehavior
	{
		private readonly IInstance factory;

		public StructureMapServiceBehavior(IInstance factory)
		{
			this.factory = factory;
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher cd = cdb as ChannelDispatcher;
				if (cd != null)
				{
					foreach (EndpointDispatcher ed in cd.Endpoints)
					{
						ed.DispatchRuntime.InstanceProvider =
							new StructureMapInstanceProvider(factory, serviceDescription.ServiceType);
					}
				}
			}
		}

		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}
	}
}
