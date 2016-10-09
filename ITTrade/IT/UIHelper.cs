using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace ITTradeUtils
{
	public static class UIHelper
	{

		public static FrameworkElement GetFirstDescendantByName(DependencyObject parent, string targetDescendantName)
		{
			FrameworkElement namedChild = null;

			int childCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);
				namedChild = child as FrameworkElement;
				if (
					namedChild != null
					&& namedChild.Name == targetDescendantName
					)
				{


					break;

				}
				else
				{
					namedChild = GetFirstDescendantByName(child, targetDescendantName);
					if (namedChild != null)
					{


						break;

					}
				}
			}

			return namedChild;
		}

	}
}
