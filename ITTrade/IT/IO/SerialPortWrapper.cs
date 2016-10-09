using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;
using ITTrade.IT;
using ITTradeUtils;


namespace IT.IO
{

	/// <summary>
	/// интеллектуальная оболочка для серийного порта
	/// </summary>
	internal class SerialPortWrapper
	{
		/// <summary>
		/// Представляет метод, который будет вызван, для отдачи полученного штрихкода
		/// </summary>
		private event BarcodeTransferedHandler barcodeTransfered;


		private void onBarcodeTransfered(String newBarcode)
		{
			if (barcodeTransfered != null)
			{
				barcodeTransfered(this, new BarcodeTransferedEventArgs(newBarcode));
			}
		}


		public SerialPortWrapper(String portName, BarcodeTransferedHandler barcodeTransferedHandler)
		{
			if (barcodeTransferedHandler == null)
			{
				throw new Exceptions.CodeLogicError("Не передан обработчик события получения штрихкода.");
			}


			try
			{
				port = new SerialPort(portName);

				port.Open();

				deleteOldBufferedPortData();

				port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

				// не предпологаю, что может быть. Для статистики.
				port.ErrorReceived += new SerialErrorReceivedEventHandler(port_ErrorReceived);

				barcodeTransfered += new BarcodeTransferedHandler(barcodeTransferedHandler);
			}
			catch(Exception ex)
			{
				Logger.Write(ex);
				MessageBox.Show("Не удалось подключится к порту сканера штрихкодов "+portName);
			}
		}



		#region deleteOldBufferedPortData

		/// <summary>
		/// очистить буфер порта от всех старых данных, имеющихся на момент подключения
		/// </summary>
		private void deleteOldBufferedPortData()
		{
			// очистить буфер порта от всех старых данных, имеющихся на момент подключения, так чтобы при подключении обработчика serialPort_DataReceived он 
			// не выполнялся тут же гарантированно можно только два раза подряд запустив serialPort.ReadExisting(). Вот такой вот баг.
			// serialPort.DiscardOutBuffer() не помогает.
			// можно вызывать, только после открытия порта
			//serialPort.DiscardOutBuffer();
			String oldData;
			// первый раз
			oldData = port.ReadExisting();
			// почему-то требуется второй раз, чтоб прочитать до конца
			oldData = port.ReadExisting();
		}

		#endregion


		private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			processBarcode();
		}


		#region processBarcode

		private void processBarcode()
		{
			string result = buffer + port.ReadExisting();

			// TODO бывает, что строка разбивается по \r\n, например "4344\r" + "\n"

			// если более 128 символов без \r\n, то это не сканер штрихкодов (обычно используются штрихкода не длиннее 64 символов)
			var group = result.Split(new String[] { "\r\n" }, StringSplitOptions.None);
			var isLong = false;
			String longStr = null;
			for (int i = 0; i < group.Length; i++)
			{
				longStr = group[i];
				if (128 < longStr.Length)
				{
					isLong = true;
					break;
				}
			}
			if (isLong)
			{
				Logger.Write("Порт \"" + PortName + "\" приислал штрихкод длиннее 128 символов: " + longStr);
				buffer = null;


				goto END;

			}

			if (group.Length == 2)
			{
				// штрихкод сформирован полностью и его нужно передавать через событие и очищать буфер
				buffer = null;
				result = result.Replace("\r\n", "");
				onBarcodeTransfered(result);
			}
			if (group.Length == 1)
			{
				// нет ни одного переноса строки, значит еще идет накопление штрихкода, тк часто он передается частями в событие
				buffer = result;
			}
			else if (2 < group.Length)
			{
				// если переносов строк несколько, то предпологаем, что система была перегружена и ничего не отправляем,
				// чтоб исключить опасность ошибок, наложений и повторных сканированний
				if (group[group.Length - 1].Length == 0)
				{
					// последний считанный штрихкод полностью передан, тк последний элемент массива пустая строка из-за того, что строка завершается переносом строки \r\n
					buffer = null;
				}
				else
				{
					// на конце не перенос строки \r\n, а это значит, что еще будет передана оставшаяся часть.
					// Чтобы не усложнять код состояниями, для последующего игнорирования штрихкода, просто порчу строку в буфере, чтоб получился несуществующий штрихкод
					buffer = "~~~~";
				}
			}

		END: ;
		}

		#endregion


		private void port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			Logger.Write(e.EventType.ToString());
		}


		private SerialPort port;

		internal string PortName
		{
			get
			{
				return port.PortName;
			}
		}


		/// <summary>
		/// тк штрихкод может передаваться частями - буферезируем его при получении
		/// </summary>
		private string buffer;


		internal void Close()
		{
			dispose();
		}


		private void dispose()
		{
			try
			{
				// может рвать для некоторых портов
				port.Dispose();
			}
			catch { }
		}
	}
}
