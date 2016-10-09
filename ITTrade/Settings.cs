using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITTrade
{
	public static class Settings
	{
		static Settings()
		{
			SoundsAllowed = true;
		}

		public static Boolean SoundsAllowed { get; set; }
	}
}
