using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ITTrade.Business;
using ITTrade.IT.WPF.UI;
using ITTradeControlLibrary;

namespace ITTrade.IT.WPF
{
	class InvoicePrinting
	{
		internal static void PrintInvoice(TradeWindow tradeWindow)
		{
			var invoice = tradeWindow.InvoicesTabControl.SelectedContent as Invoice;
			if (invoice == null)
			{
				MessageWindow.Show(new UniversalMessageContext { Content = "Не выбрана накладная для печати", WindowOwnerForCenteringOrNull = tradeWindow });

				return;
			}
			if (invoice.Products.Count == 0)
			{
				MessageWindow.Show(new UniversalMessageContext { Content = "Накладная для печати пуста.", WindowOwnerForCenteringOrNull = tradeWindow });

				return;
			}
			if (invoice.InvoiceAmount == 0)
			{
				MessageWindow.Show(new UniversalMessageContext { Content = "Накладная для печати имеет нулевую сумму \"итого\".", WindowOwnerForCenteringOrNull = tradeWindow });

				return;
			}

			var flowDocument = new FlowDocument
			{
				ColumnWidth = 400,
				PagePadding = new Thickness(50),
			};

			//var flowHeadeer = new Paragraph
			//{
			//    Margin = new Thickness(0, 0, 0, 0),
			//};
			//flowDocument.Blocks.Add(flowHeadeer);
			//flowHeadeer.Inlines.Add("Накладная");

			var tableHeaderContainer = new BlockUIContainer(GetInvoiceTableHeader());
			flowDocument.Blocks.Add(tableHeaderContainer);

			// Печать таблицы накладной
			var invoiceTable = new Table
			{
				Margin = new Thickness(0, 2, 0, 0),
			};
			flowDocument.Blocks.Add(invoiceTable);

			var tableRowGroup = SetInvoiceTable(invoiceTable);

			var rowCount = 1;
			for (int i = 0; i < 4; i++)
			{
				foreach (var product in invoice.Products.Where(el => 0 < el.ProductAmount).OrderBy(el => el.ProductGroupName).OrderBy(el => el.ProductBarcode))
				{
					var currentRow = new TableRow();
					tableRowGroup.Rows.Add(currentRow);

					//if (rowCount % 2 == 0)
					//{
					//    currentRow.Background = Brushes.LightGoldenrodYellow;
					//}

					currentRow.Cells.Add(new TableCell(new Paragraph(new Run(rowCount++.ToString())))
					{
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(0, 0, 1, 1),
					});
					currentRow.Cells.Add(new TableCell(new Paragraph(new Run(product.ProductBarcode)))
					{
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(0, 0, 1, 1),
					});
					currentRow.Cells.Add(new TableCell(new Paragraph(new Run(product.ProductGroupName)))
					{
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(0, 0, 1, 1),
					});
					currentRow.Cells.Add(new TableCell(new Paragraph(new Run(product.CurrentSalePrice.ToString())))
					{
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(0, 0, 1, 1),
					});
					currentRow.Cells.Add(new TableCell(new Paragraph(new Run(product.SaleQuantity.ToString())))
					{
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(0, 0, 1, 1),
					});
					currentRow.Cells.Add(new TableCell(new Paragraph(new Run(product.ProductAmount.ToString())))
					{
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(0, 0, 0, 1),
					});
				}

			}
			// Печать дна.
			var run = new Run(
				"Итого: " + invoice.InvoiceAmount + " руб."
				);
			var bold = new Bold(run);
			var bottomParagraph = new Paragraph(bold)
			{
				Margin = new Thickness(10, 10, 10, 0),
			};
			flowDocument.Blocks.Add(bottomParagraph);

			var printDialog = new PrintDialog();
			printDialog.PrintDocument(
				new DocumentPaginatorWithPageManipulations(
					flowDocument,
					pageManipulationsData =>
					{
						var dockPanel = new DockPanel
						{
							LastChildFill = false,
						};
						pageManipulationsData.RootElement.Children.Add(dockPanel);

						if (pageManipulationsData.PageNumber == 0)
						{
							PrepareFirstPagePrinting(pageManipulationsData, dockPanel);
						}
						else
						{
							PrepareNotFirstPagePrinting(pageManipulationsData, dockPanel);
						}

					}
					),
				"Печать накладной."
				);
		}

		private static void PrepareFirstPagePrinting(DocumentPaginatorWithPageManipulations.ManipulationsData pageManipulationsData, DockPanel dockPanel)
		{
			var headerStackPanel = new StackPanel
			{
				Margin = new Thickness(50, 26, 50, 0),
			};
			dockPanel.Children.Add(headerStackPanel);
			headerStackPanel.SetValue(DockPanel.DockProperty, Dock.Top);
			headerStackPanel.Children.Add(
				new TextBlock(new Run("Накладная"))
				{
					FontWeight = FontWeights.Bold,
					FontSize = 20
				});

			var footerStackPanel = new StackPanel
			{
				Margin = new Thickness(15, 5, 15, 5),
			};
			dockPanel.Children.Add(footerStackPanel);
			footerStackPanel.SetValue(DockPanel.DockProperty, Dock.Bottom);
			footerStackPanel.Children.Add(new TextBlock(
				//new Run("Дно")
				));
		}

		// ReSharper disable UnusedParameter.Local
		private static void PrepareNotFirstPagePrinting(DocumentPaginatorWithPageManipulations.ManipulationsData pageManipulationsData, DockPanel dockPanel)
		// ReSharper restore UnusedParameter.Local
		{
			var headerStackPanel = new StackPanel
			{
				Margin = new Thickness(50, 30, 50, 0),
			};
			dockPanel.Children.Add(headerStackPanel);
			headerStackPanel.SetValue(DockPanel.DockProperty, Dock.Top);
			//headerStackPanel.Children.Add(new TextBlock(new Run("Верх")));
			headerStackPanel.Children.Add(GetInvoiceTableHeader());

			var footerStackPanel = new StackPanel
			{
				Margin = new Thickness(15, 5, 15, 5),
			};
			dockPanel.Children.Add(footerStackPanel);
			footerStackPanel.SetValue(DockPanel.DockProperty, Dock.Bottom);
			footerStackPanel.Children.Add(new TextBlock(
				//new Run("Дно")
				));
		}

		private static Grid GetInvoiceTableHeader()
		{
			var grid = new Grid();

			grid.ColumnDefinitions.Add(new ColumnDefinition
			{
				Width = new GridLength(52),

			});
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(152) });
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(102) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(102) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(102) });

			var rowNumCell = new TextBlock
			{
				Text = "№",
				FontSize = 14,
				FontFamily = new FontFamily("Verdana"),
			};
			Grid.SetColumn(rowNumCell, 0);
			grid.Children.Add(rowNumCell);

			var codeCell = new TextBlock
			{
				Text = "Код",
				FontSize = 14,
				FontFamily = new FontFamily("Verdana"),
			};
			Grid.SetColumn(codeCell, 1);
			grid.Children.Add(codeCell);

			var groupNameCell = new TextBlock
			{
				Text = "Группа",
				FontSize = 14,
				FontFamily = new FontFamily("Verdana"),
			};
			Grid.SetColumn(groupNameCell, 2);
			grid.Children.Add(groupNameCell);

			var currentSalePriceCell = new TextBlock
			{
				Text = "Цена",
				FontSize = 14,
				FontFamily = new FontFamily("Verdana"),
			};
			Grid.SetColumn(currentSalePriceCell, 3);
			grid.Children.Add(currentSalePriceCell);

			var saleQuantityCell = new TextBlock
			{
				Text = "Кол.",
				FontSize = 14,
				FontFamily = new FontFamily("Verdana"),
			};
			Grid.SetColumn(saleQuantityCell, 4);
			grid.Children.Add(saleQuantityCell);

			var productAmountCell = new TextBlock
			{
				Text = "Всего",
				FontSize = 14,
				FontFamily = new FontFamily("Verdana"),
			};
			Grid.SetColumn(productAmountCell, 5);
			grid.Children.Add(productAmountCell);

			return grid;
		}

		/// <summary>
		/// Задает общие параметры для таблицы накладной. Нужен для создания нескольких таблиц одного формата.
		/// </summary>
		/// <param name="invoiceTable"></param>
		/// <returns></returns>
		private static TableRowGroup SetInvoiceTable(Table invoiceTable)
		{
			invoiceTable.CellSpacing = 2;

			var rowCountColumn = new TableColumn
			{
				Width = new GridLength(50),
			};
			invoiceTable.Columns.Add(rowCountColumn);

			var productBarcodeColumn = new TableColumn
			{
				Width = new GridLength(150),
			};
			invoiceTable.Columns.Add(productBarcodeColumn);

			var productGroupNameColumn = new TableColumn();
			invoiceTable.Columns.Add(productGroupNameColumn);

			var currentSalePriceColumn = new TableColumn
			{
				Width = new GridLength(100),
			};
			invoiceTable.Columns.Add(currentSalePriceColumn);

			var saleQuantityColumn = new TableColumn
			{
				Width = new GridLength(100),
			};
			invoiceTable.Columns.Add(saleQuantityColumn);

			var productAmountColumn = new TableColumn
			{
				Width = new GridLength(100),
			};
			invoiceTable.Columns.Add(productAmountColumn);

			var tableRowGroup = new TableRowGroup();
			invoiceTable.RowGroups.Add(tableRowGroup);
			return tableRowGroup;
		}
	}
}
