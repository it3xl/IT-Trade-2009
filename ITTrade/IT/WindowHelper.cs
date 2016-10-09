using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ITTradeUtils
{
	public static class WindowHelper
	{

		public static Window GetWindow(this DependencyObject dependencyObject)
		{
			return Window.GetWindow(dependencyObject);
		}


		/// <summary>
		/// Доступно только в потоке из Dispatcher
		/// </summary>
		/// <returns></returns>
		public static Window GetActive()
		{
			Window windowReceiver = null;
			foreach (Window window in Application.Current.Windows)
			{
				if (window.IsActive)
				{
					windowReceiver = window;

					break;
				}
			}

			return windowReceiver;
		}

	}
}
