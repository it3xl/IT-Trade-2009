using System;

namespace ITTrade.IT.WPF.Barcode
{
	internal static class BarcodeTypeUtils
	{
		private const String InvoicePrefix = "# Invoice:";
		private const String CommandPrefix = "# Command:";
		private const String SellerPrefix = "# Seller:";
		private const String CustomerPrefix = "# Customer:";



		internal static BarcodeType GetBarcodeType(String barcode)
		{
			if(String.IsNullOrEmpty(barcode))
			{

				return BarcodeType.Unknown;
			}

			// если не найдем дальше зарегестрированный префикс, то будем рассматриваем штрихкод, как штрихкод товара
			var res = BarcodeType.Product;

			if (barcode.StartsWith(InvoicePrefix))
			{
				res = BarcodeType.Invoice;
			}
			else if (barcode.StartsWith(CommandPrefix))
			{
				res = BarcodeType.Command;
			}
			else if (barcode.StartsWith(SellerPrefix))
			{
				res = BarcodeType.Seller;
			}
			else if (barcode.StartsWith(CustomerPrefix))
			{
				res = BarcodeType.Customer;
			}

			return res;
		}
	}
}
