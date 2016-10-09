using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using ITTrade;


namespace IT.IO
{
	/// <summary>
	/// Управляет всеми сирийными портами и сканерами, подключенными к ним
	/// </summary>
	internal static class SerialPortManager
	{

		/*
		 * Основная логика - подключаеся к портам, которые указаны в аппаратных настрйках приложения.
		 * Регестрируем обработчик, который следит за изменением аппаратных настроек и переподключаем в нем порты.
		 * 
		 * Корректный штрихкод, это небольшое количество символов латиницы или цифр, заканчивающееся переносом строки /r/n.
		 * 
		 * Один раз подключивщись, держим порты до смены аппаратных настроек приложения.
		 * 
		 */

		/// <summary>
		/// храним все порты, чтоб потом переподключиться, в случау смены аппаратных настроек приложения
		/// </summary>
		private static Dictionary<String, SerialPortWrapper> grabedPorts=new Dictionary<string,SerialPortWrapper>();

		private static volatile Boolean isGrabbingActive;

		/// <summary>
		/// Помогает избежать параллельного выполнения взаимоисключающих действий
		/// </summary>
		private static Object lockerForChangeActivity = new object();

		#region BarcodeReceiver

		internal static BarcodeTransferedHandler __barcodeReceiver;
		internal static BarcodeTransferedHandler BarcodeReceiver
		{
			get
			{
				return __barcodeReceiver;
			}
			set
			{
				if (__barcodeReceiver != null)
				{
					throw new Exceptions.CodeLogicError("Только один раз за все время существования экземпляра приложения можно устанавлисать BarcodeReceiver");
				}

				__barcodeReceiver = value;
			}
		}

		#endregion


		static SerialPortManager()
		{
			ApparatSettings.ApparatSettingsUpdated += new ApparatSettings.UpdatedEventHandler(ApparatSettings_Updated);
		}


		static void ApparatSettings_Updated(object sender, EventArgs e)
		{
			unmountPorts();
			MountPorts();
		}


		#region MountPorts

		/// <summary>
		/// Подключится к указанным в приложении портам сканера штрихкода. Если уже подключен, то ничего не делаем
		/// </summary>
		internal static void MountPorts()
		{
			lock (lockerForChangeActivity)
			{
				if (isGrabbingActive == true)
				{
					// игнорируем повторные запуски граббинга, если, он уже запущен

					return;
				}
				else
				{
					isGrabbingActive = true;
				}

				foreach (String potrName in ApparatSettings.Current.Rs232BarcodeScanersUsedComPorts)
				{
					SerialPortWrapper portWrapper = new SerialPortWrapper(potrName, BarcodeReceiver);
					grabedPorts.Add(potrName, portWrapper);
				}
			}
		}

		#endregion


		#region unmountPorts

		/// <summary>
		/// Закроет порты и очистит список подключенных портов
		/// </summary>
		private static void unmountPorts()
		{
			lock (lockerForChangeActivity)
			{
				if (isGrabbingActive == false)
				{
					// игнорируем повторные остановы граббинга портов, если он уже остановлен

					return;
				}
				else
				{
					isGrabbingActive = false;
				}

				foreach (KeyValuePair<String,SerialPortWrapper> keyVal in grabedPorts)
				{
					keyVal.Value.Close();
				}

				// удалим старые порты, чтоб переподключится при необходимости
				grabedPorts.Clear();
			}
		}

		#endregion


	}
}
