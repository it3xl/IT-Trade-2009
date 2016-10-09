using System;
using System.Windows;
using IT;
using IT.IO;
using ITTradeUtils;

namespace ITTrade.IT.WPF.Barcode
{
	/// <summary>
	/// Получает штрихкоды из любого источника с любым типом подключения при помощи посредников подключений.
	/// Передает штрихкод только активному окну приложения, и только если оно подписано на получение.
	/// Если активное окно подписано на получение штрихкода, то передаст ему штрихкод
	/// По просьбе окна, исключает его из получателеый штрихкода.
	/// </summary>
	public static class BarcodeHelper
	{
		public delegate void ProcessReceivedBarcodeDelegate(string barcode);

		private static readonly Object _keyForProcessReceivedBarcodeHandler = new object();

		private static readonly Object _windowRegisteredKey = new Object();

		static BarcodeHelper()
		{
			SerialPortManager.BarcodeReceiver = ProcessReceivedBarcode;
		}

		/// <summary>
		/// Единый метод, который получает штрихкоды со всех устройств и передает их активному, зарегестрированному окну
		/// </summary>
		private static void ProcessReceivedBarcode(Object sender, BarcodeTransferedEventArgs e)
		{
			// Выполняется вне потока окна



			Application.Current.Dispatcher.BeginInvoke(
				new Action<String>(
					delegate(String barcode) {
						WindowAndHanndler windowHanndler = ActiveWindowAndBarcodeReceivingHandler;
						if (windowHanndler.BarcodeReceivingHandler == null)
						{
							// окно не зарегестрированно

							return;
						}

						// вызовем в гуевом потоке зарегестрированный обработчик окна
						windowHanndler.BarcodeReceivingHandler(barcode);
					}),
					e.Barcode);
		}


		#region activeWindowAndBarcodeReceivingHandler

		/// <summary>
		/// null никогда не будет возвращен
		/// </summary>
		/// <returns></returns>
		private static WindowAndHanndler ActiveWindowAndBarcodeReceivingHandler
		{
			get
			{
				var windowReceiver = WindowHelper.GetActive();
				if (windowReceiver == null)
				{
					// нет активного окна

					return WindowAndHanndler.Empty;
				}

				var barcodeReceivingHandler = (ProcessReceivedBarcodeDelegate)windowReceiver.Resources[_keyForProcessReceivedBarcodeHandler];
				return new WindowAndHanndler(barcodeReceivingHandler);
			}
		}

		#endregion


		#region class WindowAndHanndler

		private class WindowAndHanndler
		{
			public WindowAndHanndler(ProcessReceivedBarcodeDelegate barcodeReceivingHandler)
			{
				BarcodeReceivingHandler = barcodeReceivingHandler;
			}


			private static readonly WindowAndHanndler _empty = new WindowAndHanndler(null);

			public static WindowAndHanndler Empty
			{
				get
				{
					return _empty;
				}
			}

			public readonly ProcessReceivedBarcodeDelegate BarcodeReceivingHandler;

			public Boolean IsWindowReceivingBarcode
			{
				get
				{
					return BarcodeReceivingHandler != null;
				}
			}
		}

		#endregion


		#region BarcodeReceiveStart
		/// <summary>
		/// программное подключение окна к получению штрихкода
		/// </summary>
		/// <param name="window"></param>
		/// <param name="barcodeReceivingHandler">должен быть передан обработчик полученного штрихкода</param>
		internal static void BarcodeReceiveStart(this Window window, ProcessReceivedBarcodeDelegate barcodeReceivingHandler)
		{

			if (window.Resources[_windowRegisteredKey] == null)
			{
				// обработчики к окну еще не цеплялись

				window.Activated += WindowActivated;
				//window.Deactivated += new EventHandler(window_Deactivated);

				window.Resources[_windowRegisteredKey] = _windowRegisteredKey;
			}


			if (barcodeReceivingHandler == null)
			{
				throw new Exceptions.CodeLogicError("Должен быть передан обработчик полученного штрихкода.");
			}

			window.Resources[_keyForProcessReceivedBarcodeHandler] = barcodeReceivingHandler;

			// если окно активно - запускаем слежение
			if (window.IsActive)
			{
				SerialPortManager.MountPorts();
			}
		}

		#endregion


		private static void WindowActivated(object sender, EventArgs e)
		{
			// Выполняется в потоке окна

			// Активация окна означает только, что когдато оно было подписано на получение штрихкода, но в данный момент оно может быть и не готово для этого

			// Нет документации, которая гарантирует, что событие активация другого окна произойдет, после деактивации.
			// Поэтому проверим, что нет зарегестрированного активного окна
			if (ActiveWindowAndBarcodeReceivingHandler.IsWindowReceivingBarcode)
			{
				SerialPortManager.MountPorts();
			}
		}


		/// <summary>
		/// программное отключение окна от получения штрихкода
		/// </summary>
		internal static void BarcodeReceivingStop(this Window window)
		{
			// Выполняется в потоке окна

			// удалим оконный обработчик получения штрихкодов
			window.Resources.Remove(_keyForProcessReceivedBarcodeHandler);
		}

	}
}
