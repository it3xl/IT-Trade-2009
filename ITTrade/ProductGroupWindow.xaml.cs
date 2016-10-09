#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ITTrade.Models;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Windows.Controls;
using System.Windows.Threading;
using IT.WPF;
using ITTradeControlLibrary;
using ITTradeUtils;

#endregion

namespace ITTrade
{
	public partial class ProductGroupWindow : Window
	{
		// Для коррекной работы требуется корректная своевременная синхронизация отображаемой колекции и Entitys

		/// <summary>
		/// Ентити, через которые работаем с базой
		/// </summary>
		ProductGroupEntities productGroupEntitys;

		/// <summary>
		/// Коллекция для отображения и биндинга.
		/// </summary>
		ObservableCollection<ProductGroup> productGroups = new ObservableCollection<ProductGroup>();


		#region ProductGroupWindow

		public ProductGroupWindow()
		{
			InitializeComponent();


			var view = CollectionViewSource.GetDefaultView(productGroups);
			view.SortDescriptions.Add(new SortDescription("DateCreation", ListSortDirection.Descending));

			var viewEditable = (IEditableCollectionView)view;
			viewEditable.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

			initDataContext();

			DataContext = productGroups;

			productGroups.CollectionChanged += new NotifyCollectionChangedEventHandler(productGroups_CollectionChanged);
		}

		#endregion

		#region initDataContext

		private void initDataContext()
		{
			productGroupEntitys = new ProductGroupEntities();

			var data = ((IListSource)(from e in productGroupEntitys.ProductGroup select e)).GetList() as IBindingList;

			productGroups.Clear();

			foreach (ProductGroup pg in data)
			{
				productGroups.Add(pg);
			}
		}

		#endregion

		#region productGroups_CollectionChanged

		void productGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (ProductGroup item in e.OldItems)
				{
					productGroupEntitys.DeleteObject(item);
				}
			}
		}

		#endregion

		#region ProductGroupsDataGrid_CellEditEnding

		private void ProductGroupsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			// получим общие параметры, неоходимые для обработки
			// новое значение необходимо выделять из элемента, тк в объекте оно еще не существует
			var newName = ((TextBox)e.EditingElement).Text;
			// избавимся от концевых пробелов, тк они запрещены
			var newCheckedName = newName.Trim();

			// текущая продуктовая группа. Все варианты получения.
			// может быть в Entity, а может и не быть
			//var currentProductGroup = (ProductGroups)e.Row.DataContext;
			// Этот вариант не подходит, тк был случай, когда на новой строке было получено null
			//var currentProductGroup = (ProductGroups)ProductGroupsDataGrid.CurrentItem;
			var currentProductGroup = (ProductGroup)ProductGroupsDataGrid.SelectedValue;

			var oldName = currentProductGroup.Name;

			if (e.EditAction == DataGridEditAction.Commit)
			{
				// команда на применение результатов редактирования

				if (newCheckedName != "")
				{
					// не пустая строка

					// определим, не повторяется ли строка
					var isValueUnique = getIsValueUnique(newCheckedName, oldName);

					// нет других строк с таким именем
					if (isValueUnique)
					{
						// зададим проверенное имя
						currentProductGroup.Name = newCheckedName;
					}
					else
					{
						// есть другая строка с таким именем
						RestoreProductGroupName(currentProductGroup);
						ShowMessageSafely("Такое название уже существует!\n\nВозвращено прежнее значение.");
					}
				}
				else
				{
					// строка уже в Entity, поэтому предупредим, о недопустимости пустого занчения
					RestoreProductGroupName(currentProductGroup);
					ShowMessageSafely("Нельзя использовать пустыю строку.\n\nДля удаления выделите строку и нажмите кнопку Delete.");
				}
			}
			else if (e.EditAction == DataGridEditAction.Cancel)
			{
				// команда отменить редактирование
				// ничего не делаем, оно и само правильно работает
			}
			else
			{
				// другого состояния у DataGridEditAction не бывает и сюда никогда не попадем
			}
		}

		#endregion

		/// <summary>
		/// если вызывать MessageBox.Show(message) напрямую в обработчике DataGrid, то DataGrid будет рвать
		/// </summary>
		/// <param name="message"></param>
		private void ShowMessageSafely(string message)
		{
			Dispatcher.BeginInvoke(
				new Action(() => MessageBox.Show(message)), DispatcherPriority.Background);
		}

		private static void RestoreProductGroupName(ProductGroup currentProductGroup)
		{
			currentProductGroup.Name = currentProductGroup.Name;
		}

		#region getIsValueUnique

		private bool getIsValueUnique(String newCheckedValue, string oldValue)
		{
			// ! Внимание, пологается, что этот метод вызывается в момент, когда еще не применены новые значения.

			newCheckedValue = newCheckedValue.ToLower();
			oldValue = oldValue.ToLower();


			var oldItems =
				from el
					in productGroups
				where
					// убедимся, что это не текущий элемент мы так не может, тк у новых элементов одинаковый ID = 0
					//el.ProductGroupID != productGroupID

					// так делать нельзя, тк в выражении допустимы только примитивные типы
					//el != currentProductGroup

				// проверим имя очередного элемента в выборке
				el.Name.ToLower() == newCheckedValue

				select el
				;

			var isValueUnique = false;
			if (oldValue != newCheckedValue)
			{
				isValueUnique = 0 == oldItems.Count();
			}
			else
			{
				isValueUnique = 1 == oldItems.Count();
			}

			return isValueUnique;
		}

		#endregion

		#region getIsValueExist

		private bool getIsValueExist(String newCheckedValue)
		{
			newCheckedValue = newCheckedValue.ToLower();

			var items =
				from el
					in productGroups
				where
					el.Name.ToLower() == newCheckedValue
				select el
				;

			var isValueExist = 0 != items.Count();

			return isValueExist;
		}

		#endregion

		#region AddNewItemButton_Click

		private void AddNewItemButton_Click(object sender, RoutedEventArgs e)
		{
			var newName = NewItemNameTextBox.Text.Trim();
			NewItemNameTextBox.Text = newName;

			if (newName == "")
			{
				// пустые значения просто игнорируем

				return;

			}


			if (
				!
				getIsValueExist(newName))
			{
				NewItemNameTextBox.Text = "";
				var productGroup = new ProductGroup();
				productGroup.Name = newName;
				productGroup.DateCreation = DateTime.Now;
				productGroups.Add(productGroup);
				productGroupEntitys.AddToProductGroup(productGroup);
			}
			else
			{
				MessageBox.Show("Такое название уже существует");
			}
		}

		#endregion


		private void ReventChangesButton_Click(object sender, RoutedEventArgs e)
		{
			initDataContext();
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			productGroupEntitys.SaveChanges();
		}
	}
}
