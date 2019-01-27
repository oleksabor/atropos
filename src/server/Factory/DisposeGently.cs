using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Factory
{
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
