using Atropos.Common.Logging;
using System;
using System.Collections.Generic;
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

		public async void Load()
		{
			await Task.Run(() => AssignValue());
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

		private T _usageLog;
		public T Value
		{
			get { return _usageLog; }
			set { Set(ref _usageLog, value); }
		}
	}

	public class DataItems<TItem> : DataItem<IEnumerable<TItem>>
	{
		public DataItems(Func<IEnumerable<TItem>> loadData)
			: base(loadData)
		{ }
	}
}
