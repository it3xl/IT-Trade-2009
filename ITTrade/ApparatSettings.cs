using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO.Ports;
using System.Windows;

namespace ITTrade
{
	public class ApparatSettings
	{
		public delegate void UpdatedEventHandler(Object sender, EventArgs e);

		public static event UpdatedEventHandler ApparatSettingsUpdated;

		private void updatedRaise()
		{
			if (ApparatSettingsUpdated != null)
			{
				ApparatSettingsUpdated(Current, new EventArgs());
			}
		}


		public static string[] PortNames
		{
			get
			{
				return SerialPort.GetPortNames();
			}
		}

		/// <summary>
		/// Выбранные порты, по которым прослущиваются сканеры штрихкодов
		/// </summary>
		public List<String> Rs232BarcodeScanersUsedComPorts = new List<string>();

		/// <summary>
		/// Имя принтера для обычной бумаги
		/// </summary>
		private string printerName;
		public string PrinterName
		{
			get
			{
				return printerName;
			}
			set
			{
				printerName = value;
			}
		}

		/// <summary>
		/// Имя принтера, печатующего штрихкода
		/// </summary>
		private string barcodePrinterName;
		public string BarcodePrinterName
		{
			get
			{
				return barcodePrinterName;
			}
			set
			{
				barcodePrinterName = value;
			}
		}

		private static XmlSerializer xmlSerializer=new XmlSerializer(typeof(ApparatSettings));

		private static readonly string apparatSettingsFileName = "ApparatSettings.settings";


		#region Current

		private static ApparatSettings current;
		public static ApparatSettings Current
		{
			get
			{
				if (current == null)
				{
					IsolatedStorageFile isf = IsolatedStorageFile.GetMachineStoreForAssembly();
					IsolatedStorageFileStream fs = new IsolatedStorageFileStream(apparatSettingsFileName, System.IO.FileMode.OpenOrCreate, isf);
					try
					{
						current = (ApparatSettings)xmlSerializer.Deserialize(fs);
					}
					catch
					{
						current = new ApparatSettings();
					}
					fs.Close();
					isf.Close();
				}

				return current;
			}
		}

		#endregion


		public void Update()
		{
			IsolatedStorageFile isf = IsolatedStorageFile.GetMachineStoreForAssembly();
			IsolatedStorageFileStream fs = new IsolatedStorageFileStream(apparatSettingsFileName, System.IO.FileMode.Create, isf);

			xmlSerializer.Serialize(fs, this);


			fs.Close();
			isf.Close();

			updatedRaise();
		}
	}
}
