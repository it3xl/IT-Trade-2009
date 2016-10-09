using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ITTrade.IT.WPF.UI
{
	class DocumentPaginatorWithPageManipulations : DocumentPaginator
	{
		public class ManipulationsData
		{
			public DocumentPage DocumentPage { get; set; }
			public Grid RootElement { get; set; }

			public DocumentPaginatorWithPageManipulations DocumentPaginatorWithPageManipulations { get; set; }

			public int PageNumber { get; set; }
		}

		readonly Action<ManipulationsData> _extrnalManipulationsHandler;

		private readonly DocumentPaginator _documentPaginator;

		public DocumentPaginatorWithPageManipulations(FlowDocument flowDocument, Action<ManipulationsData> extrnalManipulationsHandler)
		{
			_documentPaginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
			_extrnalManipulationsHandler = extrnalManipulationsHandler;
		}

		public override DocumentPage GetPage(int pageNumber)
		{
			if(_extrnalManipulationsHandler ==null)
			{
				return _documentPaginator.GetPage(pageNumber);
			}

			var page = _documentPaginator.GetPage(pageNumber);

			var rootVisual = new ContainerVisual();
			rootVisual.Children.Add(page.Visual);

			var rootElement = new Grid();

			_extrnalManipulationsHandler(new ManipulationsData()
			{
				DocumentPage = page,
				DocumentPaginatorWithPageManipulations = this,
				PageNumber=pageNumber,
				RootElement = rootElement,
			});

			//rootElement.Children.Add(new TextBlock(new Run("Произвольный тестовый текст!!!")));
			var pageSize = page.Size;
			rootElement.Measure(pageSize);
			rootElement.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
			rootVisual.Children.Add(rootElement);

			var resultPage = new DocumentPage(rootVisual);

			return resultPage;
		}

		public override bool IsPageCountValid
		{
			get
			{
				return _documentPaginator.IsPageCountValid;
			}
		}

		public override int PageCount
		{
			get
			{
				return _documentPaginator.PageCount;
			}
		}

		public override Size PageSize
		{
			get
			{
				return _documentPaginator.PageSize;
			}
			set
			{
				_documentPaginator.PageSize = value;
			}
		}

		public override IDocumentPaginatorSource Source
		{
			get
			{
				return _documentPaginator.Source;
			}
		}
	}
}
