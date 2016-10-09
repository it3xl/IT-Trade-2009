using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Neodynamic.WPF;
using IT.WPF.Barcode;
using System.Windows;

namespace IT.WPF.ValidationRules
{
	class BarcodeValidationRule : ValidationRule
		//:  ValidationRuleWithValidationGroup
	{
		private static readonly string mess1 = "Штрихкод должен быть длиннее 4 символов";
		private const string mess2 = "Такое значение штрихкода не подходит";
		private const string mess3 = "Штрихкод не должен начинаться или заканчиваться пробелами";
		private const string mess4 = "Буквы в штрихкоде могут быть только в верхнем регистре";

		/// <summary>
		/// Замечено, что штрихкода короче 4 символов не читаются в Barcode 128
		/// </summary>
		private const int minBarcode128Length = 4;

		private static BarcodeProfessional barcoderInstans;

		ValidationResult validationResult;


		static BarcodeValidationRule()
		{
			mess1 = "Штрихкод должен быть длиннее "+minBarcode128Length+" символов";

			barcoderInstans = new BarcodeProfessional();
			barcoderInstans.Symbology = Symbology.Code128;
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			var barcode = (string)value;
			validationResult = GetIsBarcodeValid(barcode);

			return validationResult;
		}

		#region GetIsBarcodeValid

		/// <summary>
		/// Для валидации из внешнего кода
		/// </summary>
		/// <param name="barcode"></param>
		/// <returns></returns>
		public static ValidationResult GetIsBarcodeValid(string barcode)
		{
			var barcodeUpper = barcode.ToUpper();

			bool isValid = false;
			string resultMess = null;
			if (barcodeUpper != barcode)
			{
				resultMess = mess4;
			}
			else if (barcodeUpper.Length != barcodeUpper.Trim().Length)
			{
				resultMess = mess3;
			}
			else if (barcodeUpper.Length < minBarcode128Length)
			{
				resultMess = mess1;
			}
			else if (
				!
				getIsBarcodeValidForSyntax(barcodeUpper))
			{
				resultMess = mess2;
			}
			else
			{
				isValid = true;
			}

			return new ValidationResult(isValid, resultMess);
		}

		#endregion

		#region getIsBarcodeValidForSyntax

		private static bool getIsBarcodeValidForSyntax(string barcode)
		{
			bool res = false;

			lock (barcoderInstans)
			{
				try
				{
					barcoderInstans.Code = barcode;

					res = barcoderInstans.IsCodeValid;
				}
				catch
				{
					res = false;
				}
			}

			return res;
		}

		#endregion

	}
}
