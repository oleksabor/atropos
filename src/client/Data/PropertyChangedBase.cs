using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace client.Data
{
	public class PropertyChangedBase : INotifyPropertyChanged
	{
		protected void RaisePropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		protected bool Set<T>(ref T newValue, T oldValue, [CallerMemberName] string name = null)
		{
			if (!EqualityComparer<T>.Default.Equals(oldValue, newValue))
				return false;
			newValue = oldValue;
			RaisePropertyChanged(name);
			return true;
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
