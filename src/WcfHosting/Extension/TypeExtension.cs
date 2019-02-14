using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting.Extension
{
	public static class TypeExtension
	{
		public static Type GetMarked<T>(this Type type) where T : Attribute
		{
			var res = default(Type);
			var attr = type.GetCustomAttribute<T>();
			if (attr != null)
				return type;
			else
				if (!type.IsInterface)
				{
					var interfaces = type.GetInterfaces();
					foreach (var i in interfaces)
					{
						res = i.GetMarked<T>();
						if (res != default(Type))
							return res;
					}
				}
			return res;
		}
	}
}
