using LinqToDB.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Listener
{
	public class Mapper<TSource, TDest>
	{
		private TypeAccessor<TSource> sourceAccessor;
		private TypeAccessor<TDest> destAccessor;

		public Mapper()
		{
			sourceAccessor = TypeAccessor.GetAccessor<TSource>();
			destAccessor = TypeAccessor.GetAccessor<TDest>();
		}

		public IEnumerable<TDest> Map(IEnumerable<TSource> src)
		{
			return src.Select(_ => Map(_));
		}

		public TDest Map(TSource src)
		{
			var dst = destAccessor.Create();
			foreach (var mmDst in destAccessor.Members)
				if (mmDst.HasSetter)
				{
					object value = null;
					MemberAccessor mmSrc = null;
					try
					{
						mmSrc = sourceAccessor[mmDst.Name];
					}
					catch (ArgumentException)
					{}

					if (src != null && mmSrc != null && mmSrc.HasGetter)
						try
						{
							value = mmSrc.GetValue(src);
							mmDst.SetValue(dst, value);
						}
						catch (Exception e)
						{
							throw new ArgumentException($"failed to set property:{sourceAccessor.Type}.{mmSrc.Name} to {destAccessor.Type}.{mmDst.Name} value:{value}", e);
						}
				}
			return dst;
		}
	}
}
