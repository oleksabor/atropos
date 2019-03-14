using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace client.Data
{
	/// <summary>
	/// holds per day usage data
	/// </summary>
	/// <seealso cref="client.Data.PropertyChangedBase" />
	[DebuggerDisplay("{Day} {Used}")]
	public class UsageWeekLog : PropertyChangedBase
	{
		public UsageWeekLog(DayOfWeek day, TimeSpan used)
		{
			_used = used;
			_day = day;
		}

		TimeSpan _used;
		public TimeSpan Used
		{
			get { return _used; }
			set { Set(ref _used, value); }
		}

		DayOfWeek _day;
		public string Day { get { return _day == DateTime.Today.DayOfWeek ? "Today" : _day.ToString(); } }

		public int UsedMinutes
		{
			get
			{
				var tm = (int)Used.TotalMinutes / 10;

				return tm;
			}
		}

		public Brush Color
		{
			get
			{
				return _day == DayOfWeek.Sunday || _day == DayOfWeek.Saturday
					? Brushes.LightCoral
					: Brushes.LightBlue;
			}
		}
	}
}
