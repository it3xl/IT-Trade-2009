using System;
using System.Reflection;
using System.Windows.Data;
using System.Windows;

namespace ITTrade.IT.WPF.ValueConverts
{
	[Obfuscation(Exclude = true)]
	[ValueConversion(typeof(Boolean), typeof(Visibility))]
	class BooleanToVisibilityConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = (Boolean?)value;
			if (val.HasValue
				&& val.Value)
			{
				return Visibility.Visible;
			}
			
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = (Visibility)value;
			if (val == Visibility.Visible)
			{
				return true;
			}
			
			return false;
		}

		#endregion
	}
}
