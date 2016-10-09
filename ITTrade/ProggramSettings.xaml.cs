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
using System.IO.Ports;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Printing;
using ITTradeControlLibrary;

namespace ITTrade
{
	public partial class ProgramSettings : Window
	{
		public ProgramSettings()
		{
			this.Loaded += new RoutedEventHandler(ProgramSettings_Loaded);

			InitializeComponent();
		}

		void ProgramSettings_Loaded(object sender, RoutedEventArgs e)
		{
			ApparatSettings apparatSettings = (ApparatSettings)SettingsRootStackPanel.DataContext;

			foreach (string portName in apparatSettings.Rs232BarcodeScanersUsedComPorts)
			{
				if (BarcodeScanerPoptNamesListBox.Items.Contains(portName))
				{
					BarcodeScanerPoptNamesListBox.SelectedItems.Add(portName);
				}
			}

		}

		private void BarcodePrinterNameTextBoxSelect_Click(object sender, RoutedEventArgs e)
		{
			PrintDialog pd = new PrintDialog();
			pd.ShowDialog();
			string printQueueName = pd.PrintQueue.Name;
			BarcodePrinterNameTextBox.Text = printQueueName;
		}

		private void PrinterNameTextBoxSelect_Click(object sender, RoutedEventArgs e)
		{
			PrintDialog pd = new PrintDialog();
			pd.ShowDialog();
			string printQueueName = pd.PrintQueue.Name;
			PrinterNameTextBox.Text = printQueueName;
		}

		private void ApplySettings_Click(object sender, RoutedEventArgs e)
		{
			ApparatSettings apparatSettings = (ApparatSettings)SettingsRootStackPanel.DataContext;
			apparatSettings.Rs232BarcodeScanersUsedComPorts.Clear();
			foreach (String portName in BarcodeScanerPoptNamesListBox.SelectedItems)
			{
				apparatSettings.Rs232BarcodeScanersUsedComPorts.Add(portName);
			}
			apparatSettings.BarcodePrinterName = BarcodePrinterNameTextBox.Text;
			apparatSettings.PrinterName = PrinterNameTextBox.Text;

			apparatSettings.Update();
		}
	}
}
