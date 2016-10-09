using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using IT;

namespace ITTrade.IT.WPF
{
	public static class ValidationUtils
	{
		public static Boolean GetElementChildsHaveErrors(DependencyObject rootElement)
		{
			var res = false;
			foreach (Object childObject in LogicalTreeHelper.GetChildren(rootElement))
			{
				var child = childObject as DependencyObject;
				if (child == null)
				{
					continue;
				}

				if (Validation.GetHasError(child))
				{
					res = true;
				}
				else
				{
					res = GetElementChildsHaveErrors(child);
				}

				if (res==true)
				{
					break;
				}
			}
			return res;
		}

		public static void GetErrorMessagesFromChildElements(StringBuilder allErrorMessages, DependencyObject rootElement)
		{
			foreach (object child in LogicalTreeHelper.GetChildren(rootElement))
			{
				var element = child as Control;
				if (element == null) continue;

				if (Validation.GetHasError(element))
				{
					foreach (ValidationError error in Validation.GetErrors(element))
					{
						if (ValidationUtils.GetHaveInnerException(error))
						{
							allErrorMessages.Append("  " + error.ErrorContent.ToString());
						}
						else
						{
							allErrorMessages.Append("  " + ValidationUtils.GetMessageFromLastInnerException(error));
						}
						allErrorMessages.Append("\r\n");
					}
				}
				GetErrorMessagesFromChildElements(allErrorMessages, element);
			}
		}

		internal static bool GetHaveInnerException(ValidationError error)
		{
			return error.Exception != null && error.Exception.InnerException != null;
		}

		internal static String GetMessageFromLastInnerException(ValidationError error)
		{
			var ex = error.Exception;
			return ExceptionUtils.GetMessageFromLastInnerException(ex);
		}
	}
}
