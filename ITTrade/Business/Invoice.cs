
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using ITTrade.IT;
using ITTradeControlLibrary;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Collections;
using Microsoft.Windows.Controls;

namespace ITTrade.Business
{
	public class Invoice : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String propertyName)
		{
			IsChanged = true;

			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}

		}

		#endregion

		DispatcherTimer _amountMonitoringDispatcherTimer;

		public Invoice(InvoiceSaleMode invoiceSaleMode)
		{
			InvoiceSaleMode = invoiceSaleMode;

			InitAmountMonitoring();
		}

		private void InitAmountMonitoring()
		{
			_amountMonitoringDispatcherTimer
				= new DispatcherTimer(
					TimeSpan.FromMilliseconds(300),
					DispatcherPriority.Background,
					DispatcherTimer_Tick,
					Application.Current.Dispatcher
				)
			;
			StartAmountMonitoring();
		}

		private void StartAmountMonitoring()
		{
			// �������������� �� �������� ������� ��������
			AttachProductsMonitoring(Products);
			// ������ ������������ ���� ����� ��������� � ���������
			Products.CollectionChanged += ProductsCollectionChanged;
		}

		void ProductsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// ���� ��� �� ������� ������������ ��������� �� ���������
			if (e.Action != NotifyCollectionChangedAction.Move)
			{
				var notifyItems = e.NewItems;
				AttachProductsMonitoring(notifyItems);
			}
		}

		private void AttachProductsMonitoring(IList notifyItems)
		{
			if (notifyItems
				!= null)
			{
				// ���� ��� ��������� ������ �� ���������, �� ����� null, ������� ������ � foreach
				foreach (INotifyPropertyChanged notifyItem in notifyItems)
				{
					// ��������� �������� �� ���������� ����� ���������
					notifyItem.PropertyChanged += Notify_PropertyChanged;
				}
			}

			// �������������� ����� ��������� �������� ���������, �� ��� �������� ����� ������ �� �������, ��� ����� ����������� Amount
			NotifyAmountChanged();
		}

		private void Notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyAmountChanged();
		}

		private void NotifyAmountChanged()
		{
			_amountMonitoringDispatcherTimer.Stop();
			_amountMonitoringDispatcherTimer.Start();
		}

		void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			_amountMonitoringDispatcherTimer.Stop();
			SetAmounts();
		}

		private void SetAmounts()
		{
			var resAmount = Products.Sum(product => product.ProductAmount);
			InvoiceAmount = resAmount;

			var positionsAmount = Products.Sum(product => product.SaleQuantity);
			PositionsAmount = positionsAmount;

			NotifyPropertyChanged("InvoiceAmount");
			NotifyPropertyChanged("PositionsAmount");
		}

		public Decimal InvoiceAmount { get; private set; }

		public Decimal PositionsAmount { get; private set; }

		public String InvoiceBarcode { get; private set; }

		public String TypeName
		{
			get
			{
				String typeName = null;
				if (InvoiceSaleMode == InvoiceSaleMode.Wholesale)
				{
					typeName = "������� ���������";
				}
				else if (InvoiceSaleMode == InvoiceSaleMode.Retail)
				{
					typeName = "��������� ���������";
				}

				return typeName;
			}
		}

		public Boolean IsWholesale
		{
			get
			{
				if (InvoiceSaleMode == InvoiceSaleMode.Wholesale)
				{
					return true;
				}
				else
				{
					return false;
				}

			}
		}

		public Boolean IsRetail
		{
			get
			{
				return !IsWholesale;
			}
		}

		public InvoiceSaleMode InvoiceSaleMode { get; private set; }

		public String SellerName { get; private set; }

		public Int64 SellerId { get; private set; }

		public String CustomerName { get; private set; }

		public Int64 CustomerId { get; private set; }

		private ObservableCollection<InvoicesProduct> _products;

		public ObservableCollection<InvoicesProduct> Products
		{
			get
			{
				if (_products == null)
				{
					_products = new ObservableCollection<InvoicesProduct>();
					StartAmountMonitoring();
				}
				return _products;
			}
		}

		internal Boolean IsChanged { get; private set; }

		/// <summary>
		/// ��������� ��� ��������� � ���� � ��������� ���� ��� �� �������� ���������
		/// </summary>
		internal Boolean IsStored { get; private set; }

		public DataGrid HostDataGrid
		{
			get; set;
		}

		/// <summary>
		/// ��������� ����� �� ���������, ���� ��� ��� ��� � ��������� ��� ����������� ��� ����������.
		/// </summary>
		internal InvoicesProduct AddProductOrIncreaseSaleQuantity(string productBarcode)
		{
			productBarcode = productBarcode.ToUpper();
			var product = Products.FirstOrDefault(el => el.ProductBarcode == productBarcode);
			if (product != null)
			{
				// ����������� ����������.
				product.IncreaseQuantity();

				return product;
			}

			var newProduct = new InvoicesProduct(productBarcode, this);
			if (newProduct.Exist == false)
			{
				Sounds.Play(TargetSound.UnknownProductBarcode);
				MessageWindow.Show(
					new UniversalMessageContext
					{
						Content = "����� �� ������!",
					}
					);

				return null;
			}
			Products.Add(newProduct);

			return newProduct;
		}

		/// <summary>
		/// ��������� ���������� ������ ��� ������� ����� �� ���������, ���� ���������� ����� 1.
		/// </summary>
		/// <param name="productBarcode"></param>
		public InvoicesProduct DecreaseProductSaleQuantityOrRemove(string productBarcode)
		{
			productBarcode = productBarcode.ToUpper();
			var product = Products.FirstOrDefault(el => el.ProductBarcode == productBarcode);

			if (product == null)
			{
				Sounds.Play(TargetSound.UnusedProductBarcode);

				return null;
			}

			if (product.SaleQuantity <= 1)
			{
				Products.Remove(product);
			}

			product.DecreaseQuantity();

			if (product.SaleQuantity == 1)
			{
				Sounds.Play(TargetSound.QuantityEqualsOne);
			}

			if (product.SaleQuantity == 0)
			{
				Sounds.Play(TargetSound.QuantityEqualsZero);
			}

			return product;
		}

		/// <summary>
		/// �������� ��������� �� ��������� ���������
		/// </summary>
		/// <param name="barcode"></param>
		/// <param name="windowOwnerForCentering"></param>
		/// <returns></returns>
		internal static Invoice GetStoredInvoiceTry(string barcode, Window windowOwnerForCentering)
		{
			// i!t! ��� �� �����������. ����� ��������.

			MessageWindow.Show(
				new UniversalMessageContext
				{
					Content = "������ �������� ��������� �� ����",
					WindowOwnerForCenteringOrNull = windowOwnerForCentering,
				}
			);
			return null;
		}
	}
}

