using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace client.Wpf
{
	//https://stackoverflow.com/questions/5974763/binding-timespan-in-wpf-mvvm-and-show-only-minutesseconds
	[ValueConversion(typeof(TimeSpan), typeof(String))]
	public class TimespanConverter : IValueConverter
	{
		const string format = @"hh\:mm";

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((TimeSpan)value).ToString(format);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// TODO something like:
			if (TimeSpan.TryParseExact(value.ToString(), format, CultureInfo.CurrentCulture, out TimeSpan result))
				return result;
			else
				return TimeSpan.Zero;
		}
	}
}
