using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionReader.Utilities
{
	public static class StringHelper
	{
		public static string StripQuotes(string text)
		{
			if (string.IsNullOrEmpty(text)) return string.Empty;
			return text.Replace("\"", "");
		}
	}
}
