using Atropos.Common.Dto;
using client.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Data
{
	public static class CurfewExtension
	{
		public static Curfew ToDto(this CurfewGui value)
		{
			return value.Value;
		}

		public static CurfewGui ToGui(this Curfew value)
		{
			return new CurfewGui(value);
		}
	}
}
