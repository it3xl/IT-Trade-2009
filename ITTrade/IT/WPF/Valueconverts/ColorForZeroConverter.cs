using System;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace ITTrade.IT.WPF.ValueConverts
{
	[Obfuscation(Exclude = true)]
	//[ValueConversion(typeof(DateTime), typeof(string))]
	public class ColorForZeroConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var resultColor = Colors.Transparent;
			var instDecimal = value as Decimal?;
			if (instDecimal.HasValue
				&& instDecimal.Value==0)
			{
				resultColor = Colors.Red;
			}
			else
			{
				var instDouble = value as Double?;
				if (instDouble.HasValue
					&& instDouble.Value == 0)
				{
					resultColor = Colors.Red;
				}
			}

			return resultColor;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
