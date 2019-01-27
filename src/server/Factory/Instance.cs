using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Factory
{
	public class Instance : IDisposable
	{
		IContainer _container;

		public Instance(IContainer container)
		{
			_container = container;
		}

		public T Create<T>()
		{
			return _container.GetInstance<T>();
		}

		public Instance Child()
		{
			return new Instance(_container.CreateChildContainer());
		}

		private bool notDisposed = true;
		public void Dispose()
		{
			if (notDisposed)
			{
				notDisposed = false;
				_container.Dispose();
			}
		}
	}
}
