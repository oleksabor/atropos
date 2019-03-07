using Atropos.Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Data
{
	public class DataItem<T> : PropertyChangedBase, INotifyDataErrorInfo
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		public DataItem(Func<T> loadData)
		{
			LoadData = loadData;
		}

		protected Func<T> LoadData;

		public Task LoadAsync()
		{
			return Task.Run(() => AssignValue());
		}

		void AssignValue()
		{
			try
			{
				errors.Clear(nameof(Value));
				Value = LoadData();
			}
			catch (Exception e)
			{
				var message = string.Format("failed to load data:{0}", typeof(T));
				Log.ErrorException(message, e);
				errors.Add(e.Message, this, nameof(Value));
				throw new ApplicationException(message, e);
			}
		}

		ErrorsChanger errors = new ErrorsChanger();

		public IEnumerable GetErrors(string propertyName)
		{
			return errors.GetErrors(propertyName);
		}

		private T _v;

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
		{
			add { errors.ErrorsChanged += value; }
			remove { errors.ErrorsChanged -= value; }
		}

		public T Value
		{
			get { return _v; }
			set { Set(ref _v, value); }
		}

		public bool HasErrors => errors.HasErrors;
	}

	public class DataItems<TItem> : DataItem<ObservableCollection<TItem>>
	{
		public DataItems(Func<ObservableCollection<TItem>> loadData)
			: base(loadData)
		{ }
	}
}
