using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace IT.WPF.UI
{
	public static class TemplateManager
	{















		// сделать поиск сквозь шаблоны


















		/// <summary>
		/// Получает корневой элемент, который хостит шаблон. Этот элемнет может не содержать сам шаблон в том случае если шаблон содержит его потомк, например ContentPresenter.
		/// </summary>
		public static TTemplateContainerType GetAncestorElement<TTemplateContainerType>(DependencyObject templateChildElement)
			where TTemplateContainerType : FrameworkElement
		{
			if (templateChildElement == null)
			{
				return null;
			}

			var parent = VisualTreeHelper.GetParent(templateChildElement);

			var result = parent as TTemplateContainerType;
			if (result != null)
			{
				return result;
			}

			return GetAncestorElement<TTemplateContainerType>(parent);

		}

		/// <summary>
		/// Получает элемент в котором лежит шаблон. Этот элемнет может быть непосредственно контейнером (корневым элементом шаблона верхнего уровня),
		/// а может быть потомком такого элемента контейнера.
		/// </summary>
		public static DependencyObject GetTemplateParent(DependencyObject templateChildElement)
		{
			if (templateChildElement == null)
			{
				// что-то пошло не так
				return null;
			}

			DependencyObject templateRootElement = GetTemplateRootElement(templateChildElement);

			DependencyObject templateParent = VisualTreeHelper.GetParent(templateRootElement);

			return templateParent;
		}

		public static DependencyObject GetTemplateRootElement(DependencyObject templateChildElement)
		{
			if (templateChildElement == null)
			{
				// что-то пошло не так
				return null;
			}

			DependencyObject child = null;
			DependencyObject parent = templateChildElement;
			do
			{
				child = parent;
				parent = LogicalTreeHelper.GetParent(child);
			} while (parent != null);
			return child;
		}
	}
}
