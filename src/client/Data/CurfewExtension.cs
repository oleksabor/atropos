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
			return new Curfew { Id = value.Id, Break = value.Break, Time = value.Time, UserId = value.UserId, WeekDay = value.WeekDay };
		}

		public static CurfewGui ToGui(this Curfew value)
		{
			return new CurfewGui { Id = value.Id, Break = value.Break, Time = value.Time, UserId = value.UserId, WeekDay = value.WeekDay };
		}
	}
}
