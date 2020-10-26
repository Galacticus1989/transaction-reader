using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionReader.Utilities.FileParser
{
	public class ItemValidationException : System.Exception
	{
		public ItemValidationException() : base() { }
		public ItemValidationException(string message) : base(message) { }

		public static void Throw(string message)
		{
			throw new ItemValidationException(message);
		}
	}
}
