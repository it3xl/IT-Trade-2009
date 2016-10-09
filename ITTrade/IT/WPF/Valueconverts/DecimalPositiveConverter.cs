using System;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;

namespace ITTrade.IT.WPF.ValueConverts
{
	[Obfuscation(Exclude = true)]
	[ValueConversion(typeof(Decimal), typeof(string))]
	public class DecimalPositiveConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var inst = value as Decimal?;
			if (inst.HasValue==false)
			{
				return String.Empty;
			}
			if (inst.Value<=0)
			{
				return String.Empty;
			}
			var res = inst.Value.ToString();
			if (res.EndsWith(",00") || res.EndsWith(".00"))
			{
				res = inst.Value.ToString("#");
			}

			return res;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var uiRes = (string)value;

			if (String.IsNullOrEmpty(uiRes))
			{
				return 0;
			}
			Decimal res;
			//NumberFormatInfo.InvariantInfo.NumberDecimalSeparator
			var decimalString = uiRes.Replace(',', '.');
			Decimal.TryParse(
				decimalString,
				NumberStyles.Number,
				CultureInfo.InvariantCulture,
				out res
				);
			if (res < 0)
			{
				res = 0;
			}

			return res;
		}

	}
}
