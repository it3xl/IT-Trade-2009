using System;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;

namespace ITTrade.IT.WPF.ValueConverts
{
	[Obfuscation(Exclude = true)]
	[ValueConversion(typeof(Double), typeof(String))]
	public class DoubleConverter : IValueConverter
	{

		public object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			var inst = value as Double?;
			if (inst.HasValue)
			{
				if (inst != 0)
				{
					return inst.Value;
				}

				return String.Empty;
			}

			return String.Empty;
		}

		public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			var uiRes = (String)value;

			if (String.IsNullOrEmpty(uiRes))
			{
				return 0;
			}

			// приведем, к допустимому здесь формату
			uiRes = uiRes
				// сделаем безразличным к запятым и точкам
				.Replace(',', '.')
				// сделаем безразличным к запятым и точкам
				.Trim()
				;

			// запретим висящие точки в начале
			if (uiRes.StartsWith("."))
			{
				return 0;
			}

			Double res;
			Double.TryParse(uiRes, out res);

			return res;
		}

	}
}
