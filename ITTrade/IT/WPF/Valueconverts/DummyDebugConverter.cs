using System;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;
using ITTrade.Business;

namespace ITTrade.IT.WPF.ValueConverts
{
	[Obfuscation(Exclude = true)]
	[ValueConversion(typeof(object), typeof(object))]
	public class DummyDebugConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//var productCurrent = ProductEditer.Current.ProductCurrent;
			//var productGroupId = productCurrent.ProductGroupId;
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//var productCurrent = ProductEditer.Current.ProductCurrent;
			//var productGroupId = productCurrent.ProductGroupId;
			return value;
		}

	}
}
