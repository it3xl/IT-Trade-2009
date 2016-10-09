using System;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;

namespace ITTrade.IT.WPF.ValueConverts
{
	[Obfuscation(Exclude = true)]
	[ValueConversion(typeof(int), typeof(string))]
	public class IntPositiveConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var inst = value as int?;
			if (inst.HasValue==false)
			{
				return String.Empty;
			}
			if (0 >= inst.Value)
			{
				return String.Empty;
			}
			return inst.Value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var uiRes = (string)value;

			if (String.IsNullOrEmpty(uiRes))
			{
				return 0;
			}
			int res;
			int.TryParse(uiRes,out res);
			if (res < 0)
			{
				res = 0;
			}

			return res;
		}

	}
}
