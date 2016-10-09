using System;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;

namespace ITTrade.IT.WPF.ValueConverts
{
	[Obfuscation(Exclude = true)]
	[ValueConversion(typeof(DateTime), typeof(string))]
	public class DateTimeConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var inst = (DateTime?)value;
			if (inst.HasValue)
			{
				return inst.Value.ToString("dd.MM.yy  HH:mm");
			}

			return String.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var uiRes = (string)value;
			var res = DateTime.Parse(uiRes);

			return res;
		}

	}
}
