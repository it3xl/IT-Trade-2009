using System;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;

namespace ITTrade.IT.WPF.ValueConverts
{
	[Obfuscation(Exclude = true)]
	[ValueConversion(typeof(Decimal), typeof(String))]
	public class DecimalConverter : IValueConverter
	{

		public object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			var inst = value as Decimal?;
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

			Decimal res;
			Decimal.TryParse(uiRes,
				// следующие два параметра настраивают, чтоб Decimal работал с точкой
				NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,

				out res);

			return res;
		}
	}
}
