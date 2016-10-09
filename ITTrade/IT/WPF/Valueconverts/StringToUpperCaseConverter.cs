using System;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;

namespace ITTrade.IT.WPF.ValueConverts
{
	[Obfuscation(Exclude = true)]
	[ValueConversion(typeof(string), typeof(string))]
	public class StringToUpperCaseConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var inst = (string)value;
			if (inst!=null)
			{
				inst = inst.ToUpper();
			}

			return inst;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var uiRes = (string)value;
			uiRes = uiRes.ToUpper();

			return uiRes;
		}

	}
}
