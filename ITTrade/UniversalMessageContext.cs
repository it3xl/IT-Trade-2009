using System;
using System.Windows;

namespace ITTrade
{
	/// <summary>
	/// Класс инкапсулирующий окно сообщения с кнопками и действиями
	/// <para>Чтобы появилась кнопка отмены, нужно чтобы IsShowCancelButton=true. Можно предать выполняемый по нажатию делегат.</para>
	/// Две кнопки действия появятся, если задать для них метод и текст
	/// </summary>
	public class UniversalMessageContext
	{
		/// <summary>
		/// <para>Обычно в реализации используется следующий подход:</para>
		/// Если это строка, то отобразиться с переносами и табуляцией
		/// <para>Если это наследник UIElement, то отобразиться, как элемент</para>
		/// От любого другого объекта будет отображена строка возврещенная методом ToString() этого объекта
		/// </summary>
		public Object Content{get;set;}

		public bool ShowInDialog { get; set; }

		public bool ShowCancelButton
		{
			get
			{
				return CancelButtonContext != null;
			}
			set
			{
				if (value)
				{
					if (CancelButtonContext == null)
					{
						CancelButtonContext = UniversalMessageButtonContext.GetDefaultCancelButtonContext();
					}
				}
				else
				{
					CancelButtonContext = null;
				}
			}
		}

		private UniversalMessageButtonContext _cancelButtonContext = UniversalMessageButtonContext.GetDefaultCancelButtonContext();
		public Action<bool?> CloseAction { get; set; }

		/// <summary>
		/// По умолчанию кнопка отмены отображается.
		/// </summary>
		public UniversalMessageButtonContext CancelButtonContext
		{
			get{return _cancelButtonContext;}
			set { _cancelButtonContext = value; }
		}

		public UniversalMessageButtonContext Button1Context { get; set; }

		public UniversalMessageButtonContext Button2Context { get; set; }

		public Window WindowOwnerForCenteringOrNull { get; set; }
	}
}
