using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Factory
{
	/// <summary>
	/// holds a data by string key, thread safe. <seealso cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
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

	/// <summary>
	/// holds bool data for string key
	/// </summary>
	/// <seealso cref="Atropos.Server.Factory.Marker{System.Boolean}" />
	public class MarkerBool : Marker<bool>
	{ }
}
