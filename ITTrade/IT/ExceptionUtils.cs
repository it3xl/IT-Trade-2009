using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IT
{
	public static class ExceptionUtils
	{
		public static string GetMessageFromLastInnerException(Exception ex)
		{
			Exception innerEx = ex.InnerException;
			while (innerEx != null)
			{
				ex = innerEx;
				innerEx = ex.InnerException;
			} 

			return ex.Message;
		}
	}
}
