using Atropos.Server.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Listener
{
	public class StructureMapInstanceProvider : IInstanceProvider
	{
		private readonly Type _serviceType;

		public StructureMapInstanceProvider(IInstance factory, Type serviceType)
		{
			Factory = factory;
			_serviceType = serviceType;
		}

		protected IInstance Factory { get; }

		public object GetInstance(InstanceContext instanceContext)
		{
			return GetInstance(instanceContext, null);
		}

		public object GetInstance(InstanceContext instanceContext, Message message)
		{
			return Factory.Create(_serviceType);
		}

		public void ReleaseInstance(InstanceContext instanceContext, object instance)
		{
			var disposable = instance as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
	}
}
