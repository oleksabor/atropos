using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Factory
{
	public class Marker<T>
	{
		ConcurrentDictionary<string, T> _items = new ConcurrentDictionary<string, T>();

		public void Clear()
		{
			_items.Clear();
		}

		public void Set(string key, T value)
		{
			_items.AddOrUpdate(key, value, (k, old) => value);
		}

		public bool Is(string key)
		{
			return _items.TryGetValue(key, out T value);
		}
	}

	public class MarkerBool : Marker<bool>
	{ }
}
