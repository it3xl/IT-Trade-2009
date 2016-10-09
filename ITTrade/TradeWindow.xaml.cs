#region using

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using IT.WPF.UI;
using ITTrade.IT;
using ITTrade.IT.WPF;
using ITTrade.IT.WPF.UI;
using ITTradeControlLibrary;
using ITTrade.Business;
using ITTrade.IT.WPF.Barcode;
using Microsoft.Windows.Controls;
using System.Collections.ObjectModel;

#endregion

namespace ITTrade
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class TradeWindow
	{
		public TradeWindow()
		{
			InitializeComponent();
			this.BarcodeReceiveStart(ProcessRecivedBarcode);

#if DEBUG
			Loaded += (o, args) =>
				{
					TestScanEmulation.Visibility = Visibility.Visible;
					TestScanEmulation2.Visibility = Visibility.Visible;
				};
#endif
		}

		// i!t!
		public enum ProductBarcodeMode
		{
			Add,
			Remove,
		}

		/// <summary>
		/// Обрабтчик полученного штрихкода
		/// </summary>
		/// <param name="barcode"></param>
		public void ProcessRecivedBarcode(string barcode)
		{
			barcode = barcode.ToUpper();
			var barcodeType = BarcodeTypeUtils.GetBarcodeType(barcode);
			switch (barcodeType)
			{
				case BarcodeType.Product:
					InvoicesProduct invoicesProduct;
					var currentInvoice = GetCurrentInvoice();
					if (ProductBarcodeActionAddRadioButton.IsChecked.HasValue == false
						|| ProductBarcodeActionAddRadioButton.IsChecked.Value)
					{
						invoicesProduct = currentInvoice.AddProductOrIncreaseSaleQuantity(barcode);
					}
					else
					{
						invoicesProduct = currentInvoice.DecreaseProductSaleQuantityOrRemove(barcode);
					}

					if (invoicesProduct != null)
					{
						Application.Current.Dispatcher.BeginInvoke(new Action(() =>
							{
								if (currentInvoice.HostDataGrid!=null)
								{
									currentInvoice.HostDataGrid.ScrollIntoView(invoicesProduct);
									currentInvoice.HostDataGrid.SelectedValue = invoicesProduct;
								}
							})
							);
					}
					break;
				case BarcodeType.Invoice:
					// передан штрихкод существующей накладной
					var tabWithInvoice = FindTabWithInvoiceByBarcode(barcode);
					var isInvoiceOpened = tabWithInvoice != null;
					if (isInvoiceOpened)
					{
						tabWithInvoice.Focus();
					}
					else
					{
						AddTabWithInvoiceBySomeBarcode(barcode);
					}
					break;
				case BarcodeType.Command:
					throw new NotImplementedException("Поддержка команд через штрихкоды не реализована.");
				default:
					MessageWindow.Show(new UniversalMessageContext { Content = "Незвестный штрихкод", WindowOwnerForCenteringOrNull = this });
					break;
			}
		}

		#region findTabWithInvoiceByBarcode

		private TabItem FindTabWithInvoiceByBarcode(string barcode)
		{
			return (from TabItem tabItem in InvoicesTabControl.Items let invoice = (Invoice)tabItem.DataContext where invoice.InvoiceBarcode == barcode select tabItem).FirstOrDefault();
		}

		#endregion

		/// <summary>
		/// Обработчик для эмуляции сканирования штрихкода. Для тестирования.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScanEmulation_Click(object sender, RoutedEventArgs e)
		{
			ProcessRecivedBarcode("1400");
			ProcessRecivedBarcode("1401");
			ProcessRecivedBarcode("1402");
			ProcessRecivedBarcode("1403");
			ProcessRecivedBarcode("1404");
			ProcessRecivedBarcode("1405");
			ProcessRecivedBarcode("1406");
			ProcessRecivedBarcode("1407");
			ProcessRecivedBarcode("1408");
			ProcessRecivedBarcode("1409");
			ProcessRecivedBarcode("1410");
			ProcessRecivedBarcode("1411");
			ProcessRecivedBarcode("1412");
			ProcessRecivedBarcode("1413");
			ProcessRecivedBarcode("1414");
			ProcessRecivedBarcode("1415");
			ProcessRecivedBarcode("1416");
			ProcessRecivedBarcode("1417");
			ProcessRecivedBarcode("1418");
		}

		private void ScanEmulation2_Click(object sender, RoutedEventArgs e)
		{
			ProcessRecivedBarcode("1417");
		}

		private Invoice GetCurrentInvoice()
		{
			var currentInvoice = (Invoice)InvoicesTabControl.SelectedContent;
			// нет накладных - создать оптовую

			// ReSharper disable ConvertIfStatementToNullCoalescingExpression
			if (currentInvoice == null)
			// ReSharper restore ConvertIfStatementToNullCoalescingExpression
			{
				// по умолчанию создаем опртовую накладную. Для добавления в розничную накладную, она должна быть уже открыта.
				currentInvoice = AddTabWithNewInvoice(InvoiceSaleMode.Wholesale);
			}

			return currentInvoice;
		}

		private static Invoice GetInvoiceFromTab(TabItem tabItem)
		{
			return (Invoice)tabItem.Content;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			ProcessMenuClickTry(e);
		}

		#region processMenuClickTry

		private void ProcessMenuClickTry(RoutedEventArgs e)
		{
			var tag = ((FrameworkElement)e.OriginalSource).Tag;
			if (tag == null)
			{
				return;
			}

			var action = MenuAddActions.NotInitedValue;
			try
			{
				action = (MenuAddActions)Enum.Parse(typeof(MenuAddActions), tag.ToString());
			}
			catch (Exception ex)
			{
				Logger.Write(ex);
			}

			if (action == MenuAddActions.NotInitedValue)
			{
				return;
			}

			ProcessMenuClick(action);
		}

		#endregion

		#region processMenuClick

		private void ProcessMenuClick(MenuAddActions action)
		{
			switch (action)
			{
				case MenuAddActions.CreateOptTab:
					AddTabWithNewInvoice(InvoiceSaleMode.Wholesale);
					break;
				case MenuAddActions.CreateRoznicaTab:
					AddTabWithNewInvoice(InvoiceSaleMode.Retail);
					break;
				case MenuAddActions.OpenManagerVindow:
					ManagerWindow.UiUtils.ShowSingletonManagerWindow();
					break;
			}
		}

		#endregion


		private void AddTabWithInvoiceBySomeBarcode(String barcode)
		{
			var storedInvoice = Invoice.GetStoredInvoiceTry(barcode, this);
			InsertTabWithInvoiceOnPage(storedInvoice);
		}

		private Invoice AddTabWithNewInvoice(InvoiceSaleMode invoiceSaleMode)
		{
			var invoice = new Invoice(invoiceSaleMode);
			InsertTabWithInvoiceOnPage(invoice);
			return invoice;
		}

		private void InsertTabWithInvoiceOnPage(Invoice invoice)
		{
			var tab = new TabItem { Header = invoice, Content = invoice };
			InvoicesTabControl.Items.Add(tab);
			tab.Focus();
		}


		private void CloseTabTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var textBlock = (DependencyObject)sender;
			var tabItem = (TabItem)InvoicesTabControl.ContainerFromElement(textBlock);

			if(tabItem.IsSelected)
			{
				CloseInvoiceTabTry(tabItem);
			}
			else
			{
				tabItem.IsSelected = true;
			}
		}

		#region closeInvoiceTabsTry

		private void CloseInvoiceTabTry(TabItem tabItem)
		{
			var invoice = GetInvoiceFromTab(tabItem);
			if (invoice.IsChanged == false)
			{
				InvoicesTabControl.Items.Remove(tabItem);
			}
			else
			{
				MessageWindow.Show(
					new UniversalMessageContext
					{
						Content = "Накладная не сохранена. Закрыть с потерей изменений?",
						Button1Context = new UniversalMessageButtonContext
						{
							Text = "Закрыть накладную",
							ClickHandler = () => InvoicesTabControl.Items.Remove(tabItem),
						},
						WindowOwnerForCenteringOrNull = this,
					}
					);
			}
		}

		#endregion

		private void DataGrid_Initialized(object sender, EventArgs e)
		{
			var grid = (DataGrid)sender;
			var invoice = (Invoice)grid.DataContext;
			DataGridColumn column;
			// спрячем колонку с неподходящей режиму продажи ценой
			if (invoice.IsWholesale)
			{
				column = (DataGridColumn)((DataGrid)sender).FindName("RetailPrice");
			}
			else
			{
				column = (DataGridColumn)((DataGrid)sender).FindName("WholesalePrice");
			}
			column.Visibility = Visibility.Collapsed;
		}

		private void RemoveFromInvoiceButton_Click(object sender, RoutedEventArgs e)
		{
			var element = (FrameworkElement)sender;
			var product = (InvoicesProduct)element.DataContext;
			var productsDataGrid = GetDataGrid(element);
			var source = ((ObservableCollection<InvoicesProduct>)productsDataGrid.ItemsSource);
			source.Remove(product);
		}

		private void AddItemButton_Click(object sender, RoutedEventArgs e)
		{
			var element = (FrameworkElement)sender;
			var product = (InvoicesProduct)element.DataContext;
			product.IncreaseQuantity();
		}

		private void RemouveItemButton_Click(object sender, RoutedEventArgs e)
		{
			var element = (FrameworkElement)sender;
			var product = (InvoicesProduct)element.DataContext;
			product.DecreaseQuantity();
		}

		private static DataGrid GetDataGrid(DependencyObject child)
		{
			var res = VisualTreeHelper.GetParent(child);
			if (res is DataGrid)
			{
				return (DataGrid)res;
			}
			return GetDataGrid(res);
		}

		private void DataGridCellTextBox_BindingError(object sender, ValidationErrorEventArgs e)
		{
			//var el = new DataGrid().ItemContainerGenerator.ContainerFromItem(e.OriginalSource);

			//var dataGridCell = TemplateManager.GetTemplateContainer<DataGridCell>(e.OriginalSource as FrameworkElement);
			//var dataGridRow = TemplateManager.GetTemplateContainer<FrameworkElement>(dataGridCell);
			//var dataGridRow2 = TemplateManager.GetTemplateContainer<DataGridRow>(dataGridRow);
			//var dataGridRow3 = TemplateManager.GetTemplateContainer<FrameworkElement>(dataGridRow2);
			//dataGridRow3 = dataGridRow3;

			if (e.Action == ValidationErrorEventAction.Added)
			{

			}
			else if (e.Action == ValidationErrorEventAction.Removed)
			{

			}
		}

		private void PrintButton_Click(object sender, RoutedEventArgs e)
		{
			InvoicePrinting.PrintInvoice(this);
		}

		private void InvoiceDataGrid_Loaded(object sender, RoutedEventArgs e)
		{
			var hostDataGrid = (DataGrid)sender;
			var invoice = (Invoice)(hostDataGrid).DataContext;
			invoice.HostDataGrid = hostDataGrid;
			var sortDescriptions =hostDataGrid.Items.SortDescriptions;
			sortDescriptions.Add(new SortDescription
			{
				PropertyName = "ProductGroupName",
			});
			sortDescriptions.Add(new SortDescription
			{
				PropertyName = "ProductBarcode",
			});
		}

		private void HandlyEnteredBarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key!=Key.Enter)
			{
				return;
			}

			var handlyEnteredBarcodeTextBox = sender as TextBox;
			GetCurrentInvoice().AddProductOrIncreaseSaleQuantity(handlyEnteredBarcodeTextBox.Text);
		}

		private void HandlyEnteredBarcodeButton_Click(object sender, RoutedEventArgs e)
		{
			var handlyEnteredBarcodeTextBox = ((FrameworkElement)((FrameworkElement)sender).Parent).FindName("HandlyEnteredBarcodeTextBox") as TextBox;
			GetCurrentInvoice().AddProductOrIncreaseSaleQuantity(handlyEnteredBarcodeTextBox.Text);
		}
	}


	#region MenuAddActions

	public enum MenuAddActions
	{
		NotInitedValue,

		CreateOptTab,
		CreateRoznicaTab,

		OpenManagerVindow,
	}

	#endregion

}
