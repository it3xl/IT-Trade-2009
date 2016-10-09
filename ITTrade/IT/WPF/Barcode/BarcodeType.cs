using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITTrade.IT.WPF.Barcode
{
	public enum BarcodeType
	{
		Unknown,

		/// <summary>
		/// Товар
		/// </summary>
		Product,
		/// <summary>
		/// Команда
		/// </summary>
		Command,
		/// <summary>
		/// Накладная
		/// </summary>
		Invoice,
		/// <summary>
		/// Продавец
		/// </summary>
		Seller,
		/// <summary>
		/// Покупатель
		/// </summary>
		Customer
	}
}
