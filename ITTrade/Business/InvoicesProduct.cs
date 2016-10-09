using System;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using ITTrade.IT;
using ITTrade.ITTradeDataSetTableAdapters;

namespace ITTrade.Business
{
	public class InvoicesProduct : INotifyPropertyChanged, IDataErrorInfo
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		private ITTradeDataSet.ProductRow _productRow;
		private ITTradeDataSet.ProductRow ProductRow
		{
			get
			{
				return _productRow;
			}
		}


		public InvoicesProduct(string productBarcode, Invoice invoice)
		{
			_invoice = invoice;
			SaleQuantity = 1;

			try
			{
				var productAdapter = new ProductTableAdapter();
				var productDataTable = productAdapter.GetDataByProductBarcode(productBarcode);
				if (0 < productDataTable.Rows.Count)
				{
					_productRow = productDataTable[0];

					Exist = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Write(ex, "Ошибка при чтении товара из базы по штрихкоду.");
				// throw не используем, т.к. пологаемся на свойство Exist.
			}
		}

		private readonly Invoice _invoice;

		#region IsWholesale IsRetail

		public Boolean IsWholesale
		{
			get
			{
				return _invoice.InvoiceSaleMode == InvoiceSaleMode.Wholesale;
			}
		}

		public Boolean IsRetail
		{
			get
			{
				return !IsWholesale;
			}
		}

		#endregion

		public String ProductBarcode { get { return ProductRow.ProductBarcode; } }

		public Int64 ProductId { get { return ProductRow.ProductID; } }

		public String ProductGroupName { get { return ProductRow.ProductGroupName; } }

		public Decimal CurrentWholesalePrice { get { return ProductRow.CurrentWholesalePrice; } }

		public Decimal CurrentRetailPrice { get { return ProductRow.CurrentRetailPrice; } }

		/// <summary>
		/// Отображает цену, соответствующую текущему режиму продаж накладной. Оптовая или розничная.
		/// </summary>
		public Decimal CurrentSalePrice
		{
			get
			{
				if (_invoice.InvoiceSaleMode == InvoiceSaleMode.Wholesale)
				{
					return ProductRow.CurrentWholesalePrice;
				}
				if (_invoice.InvoiceSaleMode == InvoiceSaleMode.Retail)
				{
					return ProductRow.CurrentRetailPrice;
				}

				throw new Exception("Цена не поддерживается! Неизвестный режим продаж.");
			}
		}

		public Boolean DiscountForbidden { get { return ProductRow.DiscountForbidden; } }

		/// <summary>
		/// Указывает, что продукт существует в базе.
		/// </summary>
		public Boolean Exist { get; private set; }

		private Int32 _saleQuantity;

		/// <summary>
		/// Количество продаваемого товара.
		/// </summary>
		public Int32 SaleQuantity
		{
			get
			{
				return _saleQuantity;
			}
			set
			{
				if (_saleQuantity != value)
				{
					_saleQuantity = value;
				}

				// оповещать об изменении свойства нужно даже если свойство не меняется, т.к. конвертер в биндинге может скрывать неверные значения
				// и оповещение позволит отрисовать корректные данные, которые увидит приложение при сохранении

				// Нотификация через диспатчер сделает удаление неправилных строк из UI. Нотификация напрямую оставит их отображенными.
				Application.Current.Dispatcher.BeginInvoke(new Action(() =>
					{
						NotifyPropertyChanged("SaleQuantity");
						NotifyPropertyChanged("ProductAmount");
					}));
			}
		}

		#region ProductAmount

		/// <summary>
		/// Выдает сумму, в зависимости от режима торговли - оптовая или розничная
		/// </summary>
		public Decimal ProductAmount
		{
			get
			{
				if (IsWholesale)
				{
					return SaleQuantity * CurrentWholesalePrice;
				}
				return SaleQuantity * CurrentRetailPrice;
			}
		}

		#endregion

		public void IncreaseQuantity()
		{
			SaleQuantity += 1;
		}

		public void DecreaseQuantity()
		{
			if (SaleQuantity == 0)
			{
				return;
			}

			SaleQuantity -= 1;
		}

		public string this[string columnName]
		{
			get
			{
				switch (columnName)
				{
					case "SaleQuantity":
						if (SaleQuantity<=0)
						{
							return "Количество продаваемого товара должно быть больше 0";
						}
						break;
				}

				return null;
			}
		}

		public string Error
		{
			get { throw new NotImplementedException(); }
		}
	}
}
