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

namespace ITTrade.Controls
{
	/// <summary>
	/// Interaction logic for TopStatus.xaml
	/// </summary>
	public partial class TopStatus : UserControl
	{
		public TopStatus()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(TopStatus_Loaded);
		}

		void TopStatus_Loaded(object sender, RoutedEventArgs e)
		{
			Window parentWindow = Window.GetWindow(this);
			parentWindow.Activated += new EventHandler(parentWindow_Activated);
			parentWindow.Deactivated += new EventHandler(parentWindow_Deactivated);
		}

		void parentWindow_Deactivated(object sender, EventArgs e)
		{
			FocusMarcker.Fill = Brushes.Gray;
		}

		void parentWindow_Activated(object sender, EventArgs e)
		{
			FocusMarcker.Fill = (Brush)this.Resources["BlueRadialGradientBrush"];
		}
	}
}
