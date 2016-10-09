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
using System.Windows.Threading;
using ITTrade;

namespace ITTradeControlLibrary
{
	/// <summary>
	/// Interaction logic for MessageWindow.xaml
	/// </summary>
	public sealed partial class MessageWindow : Window
	{
		public MessageWindow()
		{
			InitializeComponent();

			Closed += MessageWindow_Closed;
			KeyDown += (s,a) =>
				{
					if(a.Key==Key.Escape)
					{
						CloseWithAction(null);
					}
				};

			RegisterAutoClosingThrouDelay();
		}

		private void RegisterAutoClosingThrouDelay()
		{
			// сделаем оложенное подключение автозакрытия
			Application.Current.Dispatcher.BeginInvoke(
				new DispatcherOperationCallback(
					messageWindowObj =>
					{
						var messageWindow = (MessageWindow)messageWindowObj;
						messageWindow.Deactivated += (sender, e) => CloseWithAction(null);

						// если к этому моменту это окно стало не активным (из-за быстрого появления других окон), то его активировать зедесь не стоит,
						// т.к. это позволяет отображать много окон с сообщениями, чтоб появление нового не закрывало предыдущие

						return null;
					}
				),
				// дождемся пока все успокоиться в приложении
				DispatcherPriority.ApplicationIdle,
				this
			);
		}

		void MessageWindow_Closed(object sender, EventArgs e)
		{
			CleareDialog();
			CloseActionInvoke();
		}

		private static TextBox GetPopupMessageTextBox()
		{
			return new TextBox
			{
				IsReadOnly = true,
				TextWrapping = TextWrapping.Wrap,
				BorderThickness = new Thickness(),
				Background = Brushes.Transparent,
				FontFamily = new FontFamily("Tahoma"),
				FontSize = 14,
			};
		}

		private void ActionButton_Click(object sender, RoutedEventArgs e)
		{
			// до очистки диалога, извлечем метод-действие
			var popupActionButton = (Button)e.Source;
			var actionMethod = (UniversalMessageButtonContext)popupActionButton.Tag;

			CloseWithAction(actionMethod);
		}

		private void CloseActionInvoke()
		{
			if (CloseAction != null)
			{
				CloseAction(DialogResultCustom);
				CloseAction = null;
			}
		}

		private bool? _dialogResultCustom;

		/// <summary>
		/// Позволяет работать на подобии как с DialogResult, но когда окно отображается или нет в режиме диалога.
		/// </summary>
		public bool? DialogResultCustom
		{
			get { return DialogMode?DialogResult: _dialogResultCustom; }
			private set
			{
				if (DialogMode)
				{
					DialogResult = value;
				}
				else
				{
					_dialogResultCustom = value;
				}
			}
		}

		/// <summary>
		/// Закрытие с очисткой состояний
		/// </summary>
		private void CloseWithAction(UniversalMessageButtonContext buttonSatae)
		{
			try
			{
				if (buttonSatae != null)
				{
					DialogResultCustom = buttonSatae.DialogResult;
					if (buttonSatae.ClickHandler != null)
					{
						buttonSatae.ClickHandler();
					}
				}
			}
			catch { }

			// закрытие из обработчика события Deactivated вызовет исключение при вызове this.Close(), поэтому обернем в try-catch.
			try
			{
				Close();
			}
			catch { }
			try
			{
				if (Owner != null)
				{
					Owner.Activate();
				}
			}
			catch { }
		}

		private void CleareDialog()
		{
			PART_ActionButton1.Visibility = Visibility.Collapsed;
			PART_ActionButton1.Tag = null;
			PART_ActionButton1.Content = null;

			PART_ActionButton2.Visibility = Visibility.Collapsed;
			PART_ActionButton2.Tag = null;

			PART_ActionButton2.Content = null;

			PART_CancelButton.Visibility = Visibility.Collapsed;
			PART_CancelButton.Tag = null;
		}

		public static Boolean? ShowDialogForWindow(Object content, Window windowOwnerForCenteringOrNull)
		{
			var messageContext = new UniversalMessageContext
			{
				Content = content,
				Button1Context = UniversalMessageButtonContext.GetDefaultOkButtonContext(),
				WindowOwnerForCenteringOrNull = windowOwnerForCenteringOrNull,
			};
			return ShowDialog(messageContext);
		}

		public static void ShowForWindow(Object content, Window windowOwnerForCenteringOrNull)
		{
			var messageContext = new UniversalMessageContext
			{
				Content = content,
				Button1Context = UniversalMessageButtonContext.GetDefaultOkButtonContext(),
				WindowOwnerForCenteringOrNull = windowOwnerForCenteringOrNull,
			};
			Show(messageContext);
		}

		public static Boolean? ShowDialog(UniversalMessageContext universalMessageContext)
		{
			universalMessageContext.ShowInDialog = true;
			if (universalMessageContext.Button1Context == null && universalMessageContext.Button2Context == null)
			{
				universalMessageContext.Button1Context = UniversalMessageButtonContext.GetDefaultOkButtonContext();
			}
			return new MessageWindow().ShowInternaly(universalMessageContext);
		}

		public static void Show(UniversalMessageContext universalMessageContext)
		{
			new MessageWindow().ShowInternaly(universalMessageContext);
		}

		private Boolean? ShowInternaly(UniversalMessageContext universalMessageContext)
		{
			CloseAction = universalMessageContext.CloseAction;

			if (universalMessageContext.WindowOwnerForCenteringOrNull != null)
			{
				Owner = universalMessageContext.WindowOwnerForCenteringOrNull;
			}
			else
			{
				WindowStartupLocation = WindowStartupLocation.CenterScreen;
			}

			var uiContent = universalMessageContext.Content as UIElement;
			if (uiContent == null)
			{
				var popupMessageTextBox = GetPopupMessageTextBox();
				popupMessageTextBox.Text = universalMessageContext.Content.ToString();
				uiContent = popupMessageTextBox;
			}
			MainPopupTextPlace.Content = uiContent;

			if (universalMessageContext.Button1Context != null)
			{
				PART_ActionButton1.Visibility = Visibility.Visible;
				PART_ActionButton1.Tag = universalMessageContext.Button1Context;
				PART_ActionButton1.Content = universalMessageContext.Button1Context.Text;
			}
			else
			{
				PART_ActionButton1.Visibility = Visibility.Collapsed;
				PART_ActionButton1.Tag = null;
				PART_ActionButton1.Content = null;
			}

			if (universalMessageContext.Button2Context != null)
			{
				PART_ActionButton2.Visibility = Visibility.Visible;
				PART_ActionButton2.Tag = universalMessageContext.Button2Context;
				PART_ActionButton2.Content = universalMessageContext.Button2Context.Text;
			}
			else
			{
				PART_ActionButton2.Visibility = Visibility.Collapsed;
				PART_ActionButton2.Tag = null;
				PART_ActionButton2.Content = null;
			}

			if (universalMessageContext.CancelButtonContext != null)
			{
				PART_CancelButton.Visibility = Visibility.Visible;
				PART_CancelButton.Tag = universalMessageContext.CancelButtonContext;
				PART_CancelButton.Content = universalMessageContext.CancelButtonContext.Text;
			}
			else
			{
				PART_CancelButton.Visibility = Visibility.Collapsed;
				PART_CancelButton.Tag = null;
				PART_CancelButton.Content = null;
			}

			// При открытии окна по клику на табе нужно дополнительно задавать фокус.
			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Focus()));

			if (universalMessageContext.ShowInDialog)
			{
				DialogMode = true;
				return ShowDialog();
			}
			Show();
			return null;
		}

		private bool DialogMode { get; set; }

		private Action<bool?> CloseAction { get; set; }
	}
}
