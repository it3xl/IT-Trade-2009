using System;
using System.Windows;
using System.Windows.Input;
using IT.WPF.Barcode;
using System.Windows.Controls;
using System.Data;
using System.Windows.Media;
using ITTrade.Business;
using ITTrade.IT.WPF.Barcode;
using Microsoft.Windows.Controls;
using System.Linq;
using ITTrade.Models;
using ITTradeControlLibrary;
using ITTradeUtils;
using ITTrade.IT.WPF;

namespace ITTrade
{
	/// <summary>
	/// Interaction logic for ManagerWindow.xaml
	/// </summary>
	public partial class ManagerWindow
	{
		public ManagerWindow()
		{
			Loaded += ManagerWindow_Loaded;

			InitializeComponent();
		}

		private void ManagerWindow_Loaded(object sender, RoutedEventArgs e)
		{
			InitProductEditerOnLoaded();
			this.BarcodeReceiveStart(ProcessRecivedBarcode);
		}

		private void InitProductGroupsData()
		{
			var productGroupsEntitys = new ProductGroupEntities();
			var productGroups = productGroupsEntitys.ProductGroup.Select(e => e).ToList();
			ProductGroupsComboBox.ItemsSource = productGroups;

			//var selectedItem = productGroups.FirstOrDefault(el => el.ProductGroupID == ProductEditer.Current.ProductCurrent.ProductGroupId);
			//if (selectedItem!=null)
			//{
			//    ProductGroupsComboBox.SelectedValue = selectedItem.ProductGroupID;
			//}
		}

		/// <summary>
		/// Обрабтчик полученного штрихкода.
		/// </summary>
		/// <param name="barcode"></param>
		public void ProcessRecivedBarcode(string barcode)
		{
			// TODO: ITretyakov: Обрабатывать штрихкоды-комманд.

			ProductEditer.Current.EditProductFromBarcode(barcode);
		}

		private void CreatrNewProduct_Click(object sender, RoutedEventArgs e)
		{
			SearchProductResultsPlace.Visibility = Visibility.Collapsed;

			ProductEditer.Current.CreateNewProductForEditSafely();
		}

		private void InitProductEditerOnLoaded()
		{
			ProductEditer.Init(
				this,
				BindDataForEditing
				);
			ProductEditer.Current.CreateNewProductForEditSafely();


			InitProductGroupsData();
		}

		private void BindDataForEditing()
		{
			RootValidationStackPanel.DataContext = ProductEditer.Current.ProductCurrent;

			switch (ProductEditer.Current.Mode)
			{
				case ProductEditer.EditMode.JustCreatedNotSaved:
					BarcodeTB.Focus();
					break;
				case ProductEditer.EditMode.Existed:
					CreateNewZakupku.Focus();
					break;
			}
		}

		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			SearcheProducts();
		}

		private void SearcheProducts()
		{
			var serchText = SearcheText.Text.Trim();
			var data = ProductEditer.Current.ProductSearchByBarcode(serchText);
			SearchProductResultsGrid.ItemsSource = data;
			if (0 < data.Count)
			{
				SearchProductResultsPlace.Visibility = Visibility.Visible;
			}
			else
			{
				MessageWindow.Show(
					new UniversalMessageContext
					{
						Content = "Ничего не найдено!",
						WindowOwnerForCenteringOrNull = this,
					}
				);
			}

			const int MaxResultsCoutn = 50;
			SearchByBarcodeAlert.Visibility = data.Count == MaxResultsCoutn ? Visibility.Visible : Visibility.Collapsed;
		}

		private void HideProductSearchResults_Click(object sender, RoutedEventArgs e)
		{
			SearchProductResultsPlace.Visibility = Visibility.Collapsed;
		}

		private void EditProductFromSearchResults_Click(object sender, RoutedEventArgs e)
		{
			SearchProductResultsPlace.Visibility = Visibility.Collapsed;

			var element = (FrameworkElement)sender;
			var dataRowView = (DataRowView)element.DataContext;
			var productRow = (ITTradeDataSet.ProductRow)dataRowView.Row;
			ProductEditer.Current.EditProductFromId(productRow.ProductID);
		}

		private void SearchProductResultsGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			if (e.AddedCells.Count == 0)
			{
				return;
			}

			SearchProductResultsPlace.Visibility = Visibility.Collapsed;

			var dataRowView = (DataRowView)e.AddedCells[0].Item;
			var productRow = (ITTradeDataSet.ProductRow)dataRowView.Row;
			ProductEditer.Current.EditProductFromId(productRow.ProductID);
		}


		private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (ValidationUtils.GetElementChildsHaveErrors(RootValidationStackPanel))
			{
				MessageWindow.ShowForWindow("До сохранения исправте ошибки во вводимых данных.", this);

				return;
			}
			ProductEditer.Current.CheckAndSave();
		}

		private static readonly Brush _saveErrorBrush = Brushes.Red;
		private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ProductEditer.Current.ProductCurrent.IsChanged
				&& ValidationUtils.GetElementChildsHaveErrors(RootValidationStackPanel) == false
				&& ProductEditer.Current.ProductCurrent.IsPurchasesHaseErrors == false
				;

			if (e.CanExecute)
			{
				if (SaveButtonPlace.BorderBrush == _saveErrorBrush)
				{
					// ReSharper disable RedundantNameQualifier
					SaveButtonPlace.ClearValue(FrameworkElement.ToolTipProperty);
					// ReSharper restore RedundantNameQualifier
					SaveButtonPlace.ClearValue(Border.BorderBrushProperty);
				}
			}
			else
			{
				if (SaveButtonPlace.BorderBrush != _saveErrorBrush)
				{
					SaveButtonPlace.ToolTip = "Не все данные верны. Проверте введенные данные.";
					SaveButtonPlace.BorderBrush = _saveErrorBrush;
				}
			}
		}

		private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ProductEditer.Current.ProductUndoChangesSafely();
		}

		private readonly Brush _canUndoBackground = Brushes.Crimson;

		private void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ProductEditer.Current.ProductCurrent.IsChanged;

			if (e.CanExecute)
			{
				if (_canUndoBackground != EditedMarckerUI.Background)
				{
					EditedMarckerUI.Background = _canUndoBackground;
					EditedMarckerUI.ToolTip = "Сделанные изменения не были сохранены.";
				}
			}
			else
			{
				if (_canUndoBackground == EditedMarckerUI.Background)
				{
					EditedMarckerUI.Background = Brushes.WhiteSmoke;
					EditedMarckerUI.ToolTip = null;
				}
			}
		}

		private void ProductDeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			MessageWindow.Show(
				new UniversalMessageContext
				{
					WindowOwnerForCenteringOrNull = this,
					Content = "Вы уверены, что хотите удалить товар?\nЭту операцию нельзя отменить.",
					Button1Context = new UniversalMessageButtonContext
					{
						Text = "Удалить",
						ClickHandler = ProductEditer.Current.ProductDelete
					},
					ShowCancelButton = true,
				}
				);

		}

		private void ProductDeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ProductEditer.Current.ProductGetCanDelete();
		}

		private void PurchaseDeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var purchase = (ProductEditer.ManagersPurchase)e.Parameter;
			ProductEditer.Current.DeletePurchaseSafely(purchase);
		}

		private void ЗакупкаDeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			var purchase = (ProductEditer.ManagersPurchase)e.Parameter;
			if (purchase == null)
			{

				// когда идет инициализация, но данные еще не привязаны
				return;
			}

			e.CanExecute = ProductEditer.Current.PurchaseGetCanDelete(purchase);
		}


		private void BarcodePrintButton_Click(object sender, RoutedEventArgs e)
		{
			PrintBarcodeFromWindow(1);
		}

		private void PrintBarcodeInContextMenuButton_Click(object sender, RoutedEventArgs e)
		{
			if (!(e.OriginalSource is Button))
			{
				// это событие установлено на родительском элементе, чтоб отлавливать нажатие на любой кнопке в контекстном меню, 
				// но когда экран маленький, в контекстном меню, сверху и снизу могут появиться кнопки прокрутки RepeatButton, 
				// которые эмулируют постоянные клики. Код следующий ниже не может привести RepeatButton к типу Button 
				// и печать от наведения на RepeatButton тоже не нужна, поэтому присекаем выполнение кода ниже, если это не тип Button.

				return;
			}

			var button = (Button)e.OriginalSource;
			Int32 copyesAmount;
			Int32.TryParse(button.Tag.ToString(), out copyesAmount);

			PrintBarcodeFromWindow(copyesAmount);
		}

		private void PrintBarcodeFromWindow(int copyesAmount)
		{
			// Несмотря на использования валидатора для управления доступностью кнопки печати трихкода, ставим эту защиту дополнительно,
			// чтоб защититься от ошибок маштабирования.
			if (ProductEditer.Current.GetIsBarcodeValid() == false)
			{
				MessageWindow.Show(
					new UniversalMessageContext { Content = "Нельзя печатать неправильный штрихкод!", WindowOwnerForCenteringOrNull = this, }
				);

				return;
			}

			BarcodePrint.Print(BarcodeTB.Text, BarcodeDensity.Value, copyesAmount);
		}


		private bool _isNeedSetFocusOnFirstByPurchaseTextBox;

		private void PurchaseCreateNew_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ProductEditer.Current.PurchaseCreateNew();

			// В этом обработчике добраться до TextBox с ценой в первой строке с помощью UIHelper.GetFirstDescendantByName(InvoiceGrid, "PriceTB")
			// невозможно, тк он еще не существует и появляется много позже.
			// Поэтому делаю в обход через глобальную переменную и событие инициализации TextBox, тк не нашел подходящего события в DataGrid.

			_isNeedSetFocusOnFirstByPurchaseTextBox = true;
		}

		// ReSharper disable InconsistentNaming
		private void PurchaseTB_Loaded(object sender, RoutedEventArgs e)
		// ReSharper restore InconsistentNaming
		{
			if (_isNeedSetFocusOnFirstByPurchaseTextBox)
			{
				_isNeedSetFocusOnFirstByPurchaseTextBox = false;


				// Пока наблюдаю поведение, похожее на виртуализацию, когда при добавлении одной строки добавляется строка то в конец, то в начало, поэтому
				// буду искать первую строку вручную и задавать фокус, не пологаясь на создаваемую строку.
				SetFocusOnFirstByingPrice();
			}
		}

		private void SetFocusOnFirstByingPrice()
		{
			Dispatcher.BeginInvoke(new Action(() =>
				{
					var priceTb = UIHelper.GetFirstDescendantByName(PurchaseGrid, "PriceTB");
					priceTb.Focus();
				}));
		}

		private void PurchaseCreateNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ProductEditer.Current.Mode != ProductEditer.EditMode.Undefined;
		}

		private void BarcodeUniqueCreate_Click(object sender, RoutedEventArgs e)
		{
			ProductEditer.Current.BarcodeSetNewUniqueBarcode();
		}

		private void BarcodeUniqueCheck_Click(object sender, RoutedEventArgs e)
		{
			ProductEditer.Current.BarcodeCheckUnique();
		}

		private void MenuItemShowEquipment_Click(object sender, RoutedEventArgs e)
		{
			var programSettings = new ProgramSettings();
			programSettings.ShowDialog();
		}

		private void AddGroupButton_Click(object sender, RoutedEventArgs e)
		{
			var pgw = new ProductGroupWindow();
			pgw.ShowDialog();
		}

		// ReSharper disable InconsistentNaming
		private void BarcodeTB_ErrorStateChanged(object sender, ValidationErrorEventArgs e)
		// ReSharper restore InconsistentNaming
		{
			BarcodePrintButton.IsEnabled = e.Action == ValidationErrorEventAction.Removed;
		}

		private void SearcheText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
			{
				return;
			}

			SearchButton.Focus();
			SearcheProducts();
		}

		private void MenuItemOpenTradeWindow_Click(object sender, RoutedEventArgs e)
		{
			UiUtils.ShowSingletonTradeWindow();
		}
	}
}
