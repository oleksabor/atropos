using Atropos.Common.Dto;
using Atropos.Common.String;
using client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Wpf.Data
{
	public class DayOfWeekGui : PropertyChangedBase
	{
		private readonly Curfew dto;
		private readonly DayOfWeek Day;

		public DayOfWeekGui(DayOfWeek day, Curfew dto)
		{
			Day = day;
			this.dto = dto;
		}

		bool Get(DayOfWeek day, Curfew dto)
		{
			return dto.WeekDay.IsWeekDay((int)day);
		}

		bool Set(DayOfWeek day, Curfew dto, bool add)
		{
			var iday = (int)day;
			var isDay = dto.WeekDay.IsWeekDay(iday);
			if (add)
			{
				if (isDay)
					return false;
				else
				{
					if (dto.WeekDay.IsEmpty())
						dto.WeekDay = $"{iday}";
					else
						dto.WeekDay = $"{dto.WeekDay},{iday}";
					return true;
				}
			}
			else
			{
				if (isDay)
				{
					dto.WeekDay = dto.WeekDay.Replace($"{iday},", "").Replace($",{iday}", "").Replace(iday.ToString(), "");
					return true;
				}
				else
					return false;
			}
		}

		public bool DaySet
		{
			get { return Get(Day, dto); }
			set { if (Set(Day, dto, value)) base.RaisePropertyChanged(); }
		}
	}
}
