using System;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;

namespace ITTrade.IT.WPF.ValueConverts
{
	/// <summary>
	/// Универсальное получение сообщения об ошибке для приема (Validation.Errors)[0].ErrorContent
	/// </summary>
	[Obfuscation(Exclude = true)]
	[ValueConversion(typeof(ValidationError), typeof(String))]
	class ValidationErrorMessageConverter : IValueConverter
	{
		#region IValueConverter Members

		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			var error = value as ValidationError;
			if (error == null)
			{
				return "Неизвестная ошибка в пользовательском интерфейсе!";
			}

			if (ValidationUtils.GetHaveInnerException(error))
			{
				return ValidationUtils.GetMessageFromLastInnerException(error);
			}
			
			return error.ErrorContent.ToString();
		}

		public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
