using Atropos.Common.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Data
{
	public class DataItem<T> : PropertyChangedBase
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
				Value = LoadData();
			}
			catch (Exception e)
			{
				var message = string.Format("failed to load data:{0}", typeof(T));
				Log.ErrorException(message, e);
				throw new ApplicationException(message, e);
			}
		}

		private T _v;
		public T Value
		{
			get { return _v; }
			set { Set(ref _v, value); }
		}
	}

	public class DataItems<TItem> : DataItem<ObservableCollection<TItem>>
	{
		public DataItems(Func<ObservableCollection<TItem>> loadData)
			: base(loadData)
		{ }
	}
}
