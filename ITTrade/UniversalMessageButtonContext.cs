using System;

namespace ITTrade
{
	public class UniversalMessageButtonContext
	{
		public String Text { get; set; }
		public Action ClickHandler { get; set; }
		public Boolean? DialogResult { get; set; }


		public static UniversalMessageButtonContext GetDefaultCancelButtonContext()
		{
			return new UniversalMessageButtonContext
			{
				Text = "Отмена",
				DialogResult = false,
			};
		}

		public static UniversalMessageButtonContext GetDefaultOkButtonContext()
		{
			return new UniversalMessageButtonContext
			{
				Text = "Ok",
				DialogResult = true,
			};
		}
	}
}