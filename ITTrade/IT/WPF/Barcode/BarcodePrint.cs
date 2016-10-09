using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Neodynamic.WPF;
using System.Windows.Shapes;
using ITTrade;

namespace IT.WPF.Barcode
{
	public static class BarcodePrint
	{











		public static void Print(String barcodeVal, double barcodeDensity, int copyCount)
		{
			if (copyCount < 1)
			{
				copyCount = 1;
			}
			if (50 < copyCount)
			{
				// чтоб было видно ошибку
				copyCount = 1;
			}

			PrintQueue barcodePrinter;
			//barcodePrinter = new PrintQueue(new LocalPrintServer(), ApparatSettings.Current.BarcodePrinterName);
			try
			{
				barcodePrinter = new PrintQueue(new PrintServer(), ApparatSettings.Current.BarcodePrinterName);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Немогу найти указанномый принтер: "+ApparatSettings.Current.BarcodePrinterName,ex);
			}


			PrintDialog pd = new PrintDialog();
			pd.PrintQueue = barcodePrinter;


			Canvas canva = new Canvas();
			canva.HorizontalAlignment = HorizontalAlignment.Left;
			//canva.ClipToBounds = true;
			//canva.Background = Brushes.Brown;
			//canva.Height = 100;
			//canva.Width = 300;







///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Штрихкод
			BarcodeProfessional barcodePro = new BarcodeProfessional();
			canva.Children.Add(barcodePro);
			// TODO перевести единицы штрихкода на миллиметры
			barcodePro.BarcodeUnit = BarcodeUnit.Millimeter;
			barcodePro.Height = 144;
			barcodePro.Width = 300;
			barcodePro.Symbology = Symbology.Code128;
			barcodePro.QuietZone = new Thickness(0, 0, 0, 0);
			
			// по умолчанию true, иначе не читается
			//barcodePro.AddChecksum = true;
			
			// !!! Плотность штрихкода
			barcodePro.BarWidth = 0.325 * barcodeDensity;

			barcodePro.BarcodeAlignment = BarcodeAlignment.Left;
			barcodePro.SetValue(Canvas.LeftProperty, 13d);
			barcodePro.SetValue(Canvas.TopProperty, -17d);
			// высота области, в которой расположен штрихкод (тут достаточно большая, тк границы сверху и снизу обрезаются белыми квадратами)
			// иначе штрихкод скукожится в середину
			barcodePro.BarHeight = 100;
			// Зададим маленький шрифт, чтоб уменьшить надписи выводимые самим контролом штрихкода, хотя их и так не видно из-за большого значения в BarHeight
			barcodePro.FontSize = 0.1;
			// без контрольной суммы сканер не считывает
			barcodePro.AddChecksum = true;
			// зададим штрихкод
			barcodePro.Code = barcodeVal;
			//barcodePro.Code = "hpr-12345";
			//barcodePro.Code = "4hpr-12";
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////







			// Ограничитель штрихкода сверху
			//<Rectangle Width="400" Height="15" Fill="White" />
			Rectangle rectTop = new Rectangle();
			canva.Children.Add(rectTop);
			rectTop.Fill = Brushes.White;
			rectTop.Width = 400;
			rectTop.Height = 15;


			// Надпись над штрихкодом
			TextBlock lable = new TextBlock();
			canva.Children.Add(lable);
			lable.Text = barcodeVal;
			lable.SetValue(Canvas.LeftProperty, 30d);
			lable.SetValue(Canvas.TopProperty, -0.02d);
			TransformGroup lableTransformGroup = new TransformGroup();
			lableTransformGroup.Children.Add(new ScaleTransform(1.0, 1.0));
			//lableTransformGroup.Children.Add(new RotateTransform(270));
			lable.LayoutTransform = lableTransformGroup;


			//TextBlock visual = new TextBlock();
			//canva.Children.Add(visual);
			//visual.Text = barcodeVal;
			//visual.FontFamily = new FontFamily(new Uri("pack://application:,,,/Resources/Fonts/code128.ttf", UriKind.Absolute), "code 128");
			//visual.FontSize = 30;
			//visual.Foreground = Brushes.Black;
			////visual.Margin = new Thickness(0, 0, 0, 0);
			//visual.SetValue(Canvas.LeftProperty, 13d);
			//TransformGroup barcodeTransformGroup = new TransformGroup();
			////visual.LayoutTransform = new ScaleTransform(10, 10);
			//barcodeTransformGroup.Children.Add(new ScaleTransform(1, 3));
			//visual.LayoutTransform = barcodeTransformGroup;


			// Ограничитиль лишнего штрихкода снизу
			Rectangle rectBott = new Rectangle();
			canva.Children.Add(rectBott);
			rectBott.Fill = Brushes.White;
			rectBott.Width = 400;
			rectBott.Height = 100;
			rectBott.SetValue(Canvas.TopProperty, 40d);


			//Size pageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);
			Size pageSize = new Size(1000d, 1000d);
			canva.Measure(pageSize);
			canva.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));


			pd.PrintTicket.CopyCount = copyCount;
			pd.PrintVisual(canva, "ITTrade Barcode - " + barcodeVal);
		}
	}
}
