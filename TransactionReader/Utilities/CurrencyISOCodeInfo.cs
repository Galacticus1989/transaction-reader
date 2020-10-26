using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionReader.Utilities
{
	/// <summary>
	/// For storing ISO 4217 formats
	/// </summary>
	public static class CurrencyISOCodeInfo
	{
		private static readonly List<string> ISOCurrencySymbols;

		public static bool ContainsCurrencyCode(string code) { return ISOCurrencySymbols.Contains(code); }

		static CurrencyISOCodeInfo()
		{
			ISOCurrencySymbols = new List<string>();

			var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
						  .Select(x => new RegionInfo(x.LCID));

			foreach (var region in regions)
				if (!ISOCurrencySymbols.Contains(region.ISOCurrencySymbol))
					ISOCurrencySymbols.Add(region.ISOCurrencySymbol);
		}
	}
}
