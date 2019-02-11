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
		//public static ISync Sync = new GuiThread();

		protected void RaisePropertyChanged(string name)
		{
			//Sync.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		protected bool Set<T>(ref T newValue, T oldValue, [CallerMemberName] string name = null)
		{
			if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
				return false;
			newValue = oldValue;
			RaisePropertyChanged(name);
			return true;
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public interface ISync
	{
		void Invoke(Action a);
	}

	//public class GuiThread : ISync
	//{
	//	public void Invoke(Action a)
	//	{
	//		var forms = Application.OpenForms;
	//		if (forms.Count > 0)
	//		{
	//			var form = forms[0];
	//			form.Invoke(a);
	//		}
	//		else
	//			a();
	//	}
	//}
}
