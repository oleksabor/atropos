using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Factory
{
	/// <summary>
	/// abstract <see cref="DisposeIt"/> allows derived classes to dispose resource
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public abstract class DisposeGently : IDisposable
	{
		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		public abstract void DisposeIt();

		public void Dispose()
		{
			if (!disposedValue)
			{
				DisposeIt();
				
				disposedValue = true;
			}
		}
		#endregion
	}
}
