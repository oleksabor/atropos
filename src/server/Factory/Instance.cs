using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Factory
{
	/// <summary>
	/// IoC container wrapper. Can create child container <see cref="Child"/> and resolve instances <see cref="Create{T}"/>
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public class Instance : IInstance
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

		public IInstance Child()
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

	public interface IInstance : IDisposable
	{
		T Create<T>();
		IInstance Child();
	}
}
