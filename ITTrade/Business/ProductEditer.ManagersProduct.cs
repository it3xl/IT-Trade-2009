using System;
using System.Linq;
using System.ComponentModel;
using IT.WPF.ValidationRules;
using System.Collections.ObjectModel;
using ITTrade.ITTradeDataSetTableAdapters;

namespace ITTrade.Business
{
	public partial class ProductEditer
	{
		/// <summary>
		/// Специальная обертка для красивой настройки валидации UI и удобной прозрачной рекции на изменения.
		/// </summary>
		public class ManagersProduct : IDataErrorInfo, INotifyPropertyChanged
		{
			private ITTradeDataSet.ProductRow ProductRow
			{
				get
				{
					return _productEditer._dataSet.Product[0];
				}
			}

			private ITTradeDataSet.ProductDataTable TableProduct
			{
				get
				{
					return _productEditer._dataSet.Product;
				}
			}

			private readonly ProductEditer _productEditer;


			public ManagersProduct(ProductEditer productEditer)
			{
				_productEditer = productEditer;

				if (_productEditer._dataSet != null)
				{
					var purchasesList = (
						from row in _productEditer._dataSet.Purchase.AsEnumerable()
						select new ManagersPurchase(
							this,
							row
							)
						).ToList();
					var purchases = new ObservableCollection<ManagersPurchase>(purchasesList);
					Purchases = purchases;
				}
				else
				{
					Purchases = new ObservableCollection<ManagersPurchase>();
				}
			}

			/// <summary>
			/// позволим UI следить за наличием изменений в объекте
			/// </summary>
			public Boolean IsChanged { get; private set; }

			internal void SetNoChanged()
			{
				IsChanged = false;
			}

			/// <summary>
			/// Открытый метод, чтоб потомки Purchase могли изменять.
			/// </summary>
			internal void SetIsChanged()
			{
				if (IsChanged)
					return;

				IsChanged = true;
				NotifyPropertyChanged("IsChanged");
			}


			internal Boolean IsPurchasesHaseErrors
			{
				get
				{
// ReSharper disable AssignNullToNotNullAttribute
					return Purchases.Any(purchase => purchase[null] != null);
// ReSharper restore AssignNullToNotNullAttribute
				}
			}


			private ObservableCollection<ManagersPurchase> _purchases;
			public ObservableCollection<ManagersPurchase> Purchases
			{
				get
				{
					//if (purchases == null)
					//{
					//    purchases = new ObservableCollection<Purchase>();
					//    purchases.CollectionChanged += new NotifyCollectionChangedEventHandler(purchases_CollectionChanged);
					//}

					return _purchases;
				}
				set
				{
					_purchases = value;
					_purchases.CollectionChanged += (sender, e)=> SetIsChanged();
				}
			}

			public String Article
			{
				get
				{
					return ProductRow.Article;
				}
				set
				{
					ProductRow.Article = value;
					NotifyPropertyChanged("Article");
				}
			}

			public Boolean CanDelete
			{
				get
				{
					return ProductRow.CanDelete;
				}
				private set
				{
					ProductRow.CanDelete = value;
					NotifyPropertyChanged("CanDelete");
				}
			}

			public DateTime CreationDate
			{
				get
				{
					return ProductRow.CreationDate;
				}
			}

			public DateTime ModifyDate
			{
				get
				{
					return ProductRow.ModifyDate;
				}
			}

			public Decimal CurrentRetailPrice
			{
				get
				{
					return ProductRow.CurrentRetailPrice;
				}
				set
				{
					ProductRow.CurrentRetailPrice = value;
					NotifyPropertyChanged("CurrentRetailPrice");
				}
			}

			public Decimal CurrentWholesalePrice
			{
				get
				{
					return ProductRow.CurrentWholesalePrice;
				}
				set
				{
					ProductRow.CurrentWholesalePrice = value;
					NotifyPropertyChanged("CurrentWholesalePrice");

					// заставим перепроверить розничную цену, тк она зависит от оптовой
					NotifyPropertyChanged("CurrentRetailPrice");
				}
			}

			public Boolean? DiscountForbidden
			{
				get
				{
					if (ProductRow.IsDiscountForbiddenNull())
					{
						return null;
					}
					return ProductRow.DiscountForbidden;
				}
				set
				{
					if (value.HasValue)
					{
						ProductRow.DiscountForbidden = value.Value;
					}
					else
					{
						ProductRow[TableProduct.DiscountForbiddenColumn] = DBNull.Value;
					}
					NotifyPropertyChanged("DiscountForbidden");
				}
			}

			public String ProductBarcode
			{
				get
				{
					return ProductRow.ProductBarcode;
				}
				set
				{
					ProductRow.ProductBarcode = value;
					NotifyPropertyChanged("ProductBarcode");
				}
			}

			public Int32 ProductGroupId
			{
				get
				{
					return ProductRow.ProductGroupID;
				}
				set
				{
					ProductRow.ProductGroupID = value;
					NotifyPropertyChanged("ProductGroupID");
				}
			}

			public Int64 ProductId
			{
				get
				{
					return ProductRow.ProductID;
				}
				private set
				{
					ProductRow.ProductID = value;
					NotifyPropertyChanged("ProductId");
				}
			}


			#region IDataErrorInfo Members

			public string this[string propertyName]
			{
				get
				{
					switch (propertyName)
					{
						case "DiscountForbidden":
							if (DiscountForbidden == null)
							{
								return "Укажите, запрещена ли скидка";
							}
							break;
						case "ProductGroupID":
							if (ProductGroupId < 0)
							{
								return "Выберите продуктовую группу";
							}
							break;
						case "ProductBarcode":
						{
							var res = BarcodeValidationRule.GetIsBarcodeValid(ProductBarcode);
							if (
								!res.IsValid)
							{
								return res.ErrorContent.ToString();
							}
						}
							break;
						case "CurrentWholesalePrice":
							if (CurrentWholesalePrice <= 0)
							{
								return "Оптовая цена должна быть больше нуля";
							}
							break;
						case "CurrentRetailPrice":
							if (CurrentRetailPrice <= 0)
							{
								return "Розничная цена должна быть больше нуля";
							}
							break;
						case "ProductGroupId":
							if (ProductGroupId < 0)
							{
								return "Для товара должна быть выбрана Продуктовая Группа!";
							}
							break;
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
				if (PropertyChanged == null)
				{
					return;
				}

				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				SetIsChanged();
			}

			#endregion

			public void Save()
			{
				ProductBarcode = ProductBarcode
					.ToUpper()
					.Trim();

				var productTableAdapter = new ProductTableAdapter();
				long? productId = ProductId;
				productTableAdapter.ProductSave(
					ref productId,
					// только в верхнем регистре
					ProductBarcode,
					ProductGroupId,
					Article,
					CurrentWholesalePrice,
					CurrentRetailPrice,
					// ReSharper disable PossibleInvalidOperationException
					// То, что ProductCurrent.DiscountForbidden имеет значение проверяется валидацией на уровне UI.
					DiscountForbidden.Value,
					// ReSharper restore PossibleInvalidOperationException
					CanDelete
					);

// ReSharper disable PossibleInvalidOperationException
				// Твердо верим, что null не получим.
				ProductId = productId.Value;
// ReSharper restore PossibleInvalidOperationException

				SavePurchases();
			}

			private void SavePurchases()
			{
				Purchases.ToList().ForEach(purchase=>
					{
						var purchaseTableAdapter = new PurchaseTableAdapter();
						long? purchaseId = purchase.PurchaseId;
						purchaseTableAdapter.PurchaseSave(
							ref purchaseId,
							ProductId,
							purchase.PurchasePrice,
							purchase.StockInTrade,
							purchase.CanDelete
							);
						// ReSharper disable PossibleInvalidOperationException
						purchase.SetPurchaseId(purchaseId.Value);
						// ReSharper restore PossibleInvalidOperationException
					});
			}

			public void SetCanDelete(bool canDelete)
			{
				CanDelete = canDelete;
			}
		}
	}
}
