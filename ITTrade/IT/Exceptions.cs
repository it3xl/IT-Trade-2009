using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IT
{
	public static class Exceptions
	{
		public class CodeLogicError : Exception
		{
			public CodeLogicError() { }


			public CodeLogicError(string message) { }


			public CodeLogicError(string message, Exception innerException) { }

		}


		public class BusinessLogicFault : Exception
		{

		}


		public class ArgumentIncorrect : Exception
		{
			public ArgumentIncorrect(string message) { }
		}


	}
}
