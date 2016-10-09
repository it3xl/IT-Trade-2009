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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Printing;

namespace TestPrinting
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			Int64 barcode = 123456789012;

			PrintBarcode(barcode);




		}


		private void PrintBarcode(Int64 barcode)
		{
			LocalPrintServer lps = new LocalPrintServer();

			PrintQueueCollection pqc = lps.GetPrintQueues();

			List<PrintQueue> barPrinters = new List<PrintQueue>();

			foreach (PrintQueue pq in pqc)
			{
				if (pq.Name.StartsWith("TSC TTP-245C"))
				{
					barPrinters.Add(pq);
				}
			}

			PrintQueue barPrinter = null;


			PrintDialog pd = new PrintDialog();

			pd.PrintQueue = barPrinters[0];
			//pd.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();

			//pd.ShowDialog();


			Canvas canva = new Canvas();


			TextBlock lable = new TextBlock();
			canva.Children.Add(lable);
			lable.Text = "N.C.";
			lable.SetValue(Canvas.LeftProperty, 110d);
			lable.SetValue(Canvas.TopProperty, 16d);
			TransformGroup lableTransformGroup = new TransformGroup();
			lableTransformGroup.Children.Add(new ScaleTransform(0.8, 0.8));
			lableTransformGroup.Children.Add(new RotateTransform(270));
			lable.LayoutTransform = lableTransformGroup;


			TextBlock visual = new TextBlock();
			canva.Children.Add(visual);

			visual.Text = barcode.ToString();

			//visual.FontFamily = new FontFamily(new Uri("Resources/Fonts/code128.ttf", UriKind.Relative), "code 128");
			visual.FontFamily = new FontFamily(new Uri("pack://application:,,,/Resources/Fonts/code128.ttf", UriKind.Absolute), "code 128");

			visual.FontSize = 30;
			visual.Foreground = Brushes.Black;
			//visual.Background = Brushes.Black;

			//visual.Margin = new Thickness(0, 0, 0, 0);

			visual.SetValue(Canvas.LeftProperty, 13d);
			TransformGroup barcodeTransformGroup = new TransformGroup();
			//visual.LayoutTransform = new ScaleTransform(10, 10);
			barcodeTransformGroup.Children.Add(new ScaleTransform(1, 3));
			visual.LayoutTransform = barcodeTransformGroup;

			//Size pageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);
			Size pageSize = new Size(1000d, 1000d);

			canva.Measure(pageSize);
			canva.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));

			pd.PrintVisual(canva, "Testig");
			//pd.PrintVisual(Testig, "Testig");

			stackPan.Children.Add(canva);
		}
	}
}
