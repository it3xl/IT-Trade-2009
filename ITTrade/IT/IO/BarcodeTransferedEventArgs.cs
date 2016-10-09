using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IT.IO
{
	public class BarcodeTransferedEventArgs
	{
		public BarcodeTransferedEventArgs(String barcode)
		{
			Barcode = barcode;
		}


		public readonly String Barcode;
	}


	public delegate void BarcodeTransferedHandler(Object sender, BarcodeTransferedEventArgs e);
}
