using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace client.Data
{
	class ErrorsChanger
	{
		class ErrorInfo
		{
			internal string Name;
			internal string Error;
		}

		Collection<ErrorInfo> errors = new Collection<ErrorInfo>();

		public void Clear([CallerMemberName] string name = null)
		{
			var toremove = errors.Where(_ => _.Name == name).ToList();
			lock (errorsLock)
			foreach (var tr in toremove)
				errors.Remove(tr);
			
		}

		object errorsLock = new object();

		public void Add(string error, object sender, [CallerMemberName] string name = null)
		{
			lock(errorsLock)
			{
				errors.Add(new ErrorInfo { Name = name, Error = error });
				ErrorsChanged?.Invoke(sender, new DataErrorsChangedEventArgs(name));
			}
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public IEnumerable GetErrors(string propertyName)
		{
			lock (errorsLock)
				return errors.Where(_ => _.Name == propertyName).ToList();
		}

		public bool HasErrors => errors.Count > 0;
	}
}
