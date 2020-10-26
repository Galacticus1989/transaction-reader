using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionReader.Models;

namespace TransactionReader.Utilities.FileParser
{
	public class CSVImporter : FileImporter
	{
		public CSVImporter(string path) : base(path) { }

		protected override string DateFormat()
		{
			return "dd/MM/yyyy hh:mm:ss";
		}

		public override FileParsingResult ParseTransactionFile()
		{
			var results = new FileParsingResult();
			try
			{
				using (var parser = new TextFieldParser(FilePath))
				{
					parser.TextFieldType = FieldType.Delimited;
					parser.SetDelimiters(",");
					while (!parser.EndOfData)
					{
						//Process row
						var fields = parser.ReadFields();
                        var rawData = new TransactionRawData
                        {
                            TransactionId = StringHelper.StripQuotes(fields[0]),
                            Amount = StringHelper.StripQuotes(fields[1]),
                            CurrencyCode = StringHelper.StripQuotes(fields[2]),
                            TransactionDate = StringHelper.StripQuotes(fields[3]),
                            Status = StringHelper.StripQuotes(fields[4])
                        };

                        var parsedItem = new Transaction();

						try
						{
							parsedItem = TransactionItemValidator.ValidateAndCreate(rawData, DateFormat());
						}
						catch (ItemValidationException ex)
						{
							results.InvalidItems.Add(rawData.TransactionId + ", " + ex.Message);
							continue;
						}
						// Add to results
						results.Results.Add(parsedItem.TransactionId, parsedItem);
					}
				}
			}
			catch (Exception e)
			{
				results.InvalidItems.Add(e.Message);
			}

			return results;
		}
	}
}
