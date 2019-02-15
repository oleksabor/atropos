﻿using Atropos.Common.Dto;
using Atropos.Common.String;
using client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Wpf.Data
{
	public class CurfewGui : Curfew
	{
		IDictionary<DayOfWeek, DayOfWeekGui> _items = new Dictionary<DayOfWeek, DayOfWeekGui>();

		public CurfewGui(Curfew value)
		{
			WeekDay = value.WeekDay;
			Time = value.Time;
			Break = value.Break;
		}

		public DayOfWeekGui this[DayOfWeek index]
		{
			get
			{
				if (!_items.ContainsKey(index))
					_items.Add(index, new DayOfWeekGui(index, this));
				return _items[index];
			}
		}

		public DayOfWeekGui Monday { get { return this[DayOfWeek.Monday]; } }
		public DayOfWeekGui Tuesday { get { return this[DayOfWeek.Tuesday]; } }
		public DayOfWeekGui Wednesday { get { return this[DayOfWeek.Wednesday]; } }
		public DayOfWeekGui Thursday { get { return this[DayOfWeek.Thursday]; } }
		public DayOfWeekGui Friday { get { return this[DayOfWeek.Friday]; } }
		public DayOfWeekGui Saturday { get { return this[DayOfWeek.Saturday]; } }
		public DayOfWeekGui Sunday { get { return this[DayOfWeek.Sunday]; } }
	}
}