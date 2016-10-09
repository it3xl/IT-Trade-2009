using System;
using System.ComponentModel;

namespace ITTrade.Business
{
	public partial class ProductEditer
	{
		/// <summary>
		/// Специальная обертка для красивой настройки валидации UI
		/// </summary>
		public class ManagersPurchase : IDataErrorInfo, INotifyPropertyChanged
		{
			internal ITTradeDataSet.PurchaseRow PurchaseRow;

			private readonly ManagersProduct _parentManagersProduct;

			public ManagersPurchase(
				ManagersProduct parentManagersProduct, 
				ITTradeDataSet.PurchaseRow purchaseRow
				)
			{
				_parentManagersProduct = parentManagersProduct;
				PurchaseRow = purchaseRow;
			}



			public Boolean CanDelete
			{
				get
				{
					return PurchaseRow.CanDelete;
				}
			}

			public DateTime CreationDate
			{
				get
				{
					return PurchaseRow.CreationDate;
				}
			}

			public DateTime ModifyDate
			{
				get
				{
					return PurchaseRow.ModifyDate;
				}
			}

			public Decimal PurchasePrice
			{
				get
				{
					return PurchaseRow.PurchasePrice;
				}
				set
				{
					PurchaseRow.PurchasePrice = value;
					NotifyPropertyChanged("PurchasePrice");
				}
			}

			public Int32 StockInTrade
			{
				get
				{
					return PurchaseRow.StockInTrade;
				}
				set
				{
					PurchaseRow.StockInTrade = value;
					NotifyPropertyChanged("StockInTrade");
				}
			}

			public Int64 PurchaseId
			{
				get
				{
					return PurchaseRow.PurchaseID;
				}
				private set
				{
					PurchaseRow.PurchaseID = value;
					NotifyPropertyChanged("PurchaseId");
				}
			}


			#region IDataErrorInfo Members

			public string this[string propertyName]
			{
				get
				{
					// чтоб сделать сквозную проверку из-за отсутствия нормальной валидации в DataGrid добавляю условие "|| String.IsNullOrEmpty(propertyName)"

					if (propertyName == "PurchasePrice" || String.IsNullOrEmpty(propertyName))
					{
						if (PurchasePrice <= 0 )
						{
							return "Закупочная цена должна быть больше нуля";
						}
					}

					// ошибок нет.
					return null;
				}
			}

			public string Error
			{
				get { throw new NotImplementedException(); }
			}

			#endregion

			#region INotifyPropertyChanged

			public event PropertyChangedEventHandler PropertyChanged;

			private void NotifyPropertyChanged(String propertyName)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
					_parentManagersProduct.SetIsChanged();
				}
			}

			#endregion

			public void SetPurchaseId(long purchaseId)
			{
				PurchaseId = purchaseId;
			}
		}
	}
}
