using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ITTrade
{
	public partial class ManagerWindow
	{
		internal static class UiUtils
		{
			private static ManagerWindow GetSingletonManagerWindow()
			{
				return Application.Current.Windows.OfType<ManagerWindow>().FirstOrDefault();
			}

			private static TradeWindow GetSingletonTradeWindow()
			{
				return Application.Current.Windows.OfType<TradeWindow>().FirstOrDefault();
			}

			internal static void ShowSingletonManagerWindow()
			{
				var managerWindow = GetSingletonManagerWindow();
				if (managerWindow != null)
				{
					managerWindow.Activate();
				}
				else
				{
					managerWindow = new ManagerWindow();
					managerWindow.Show();
				}
			}

			internal static void ShowSingletonTradeWindow()
			{
				var tradeWindow = GetSingletonTradeWindow();
				if (tradeWindow != null)
				{
					tradeWindow.Activate();
				}
				else
				{
					tradeWindow = new TradeWindow();
					tradeWindow.Show();
				}
			}
		}
	}
}