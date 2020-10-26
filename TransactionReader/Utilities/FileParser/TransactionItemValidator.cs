using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TransactionReader.Models;

namespace TransactionReader.Utilities.FileParser
{
	public class TransactionItemValidator
	{
		private const int TRANSACTIONID_MAX_LENGTH = 50;

		public static Transaction ValidateAndCreate(TransactionRawData data, string dateFormat)
		{
			if (data == null) ItemValidationException.Throw("Data is null, cannot validate");
			if (data.TransactionId.Length > TRANSACTIONID_MAX_LENGTH) ItemValidationException.Throw($"Id is longer than {TRANSACTIONID_MAX_LENGTH}");

			decimal amount;
			// Check decimal is valid
			if (!decimal.TryParse(data.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
			{
				ItemValidationException.Throw("Amount is not decimal");
			}

			if (!CurrencyISOCodeInfo.ContainsCurrencyCode(data.CurrencyCode))
			{
				ItemValidationException.Throw("Currency Code is not correct");
			}

			// Date
			var dateResult = DateTime.Now;
			var provider = CultureInfo.InvariantCulture;
			try
			{
				// Check against format
				dateResult = DateTime.ParseExact(data.TransactionDate, dateFormat, provider);
			}
			catch
			{
				ItemValidationException.Throw("Date is not in the correct format.");
			}

			var status = TransactionStatus.GetStatusCode(data.Status);
			if (string.IsNullOrEmpty(status))
			{
				ItemValidationException.Throw("Status is incorrect");
			}

			return new Transaction
			{
				TransactionId = data.TransactionId,
				Amount = amount,
				CurrencyCode = data.CurrencyCode,
				TransactionDate = dateResult,
				Status = status
			};
		}
	}
}
