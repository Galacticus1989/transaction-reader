using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TransactionReader.Models;

namespace TransactionReader.Utilities.FileParser
{
	public class XMLImporter : FileImporter
	{
		protected override string DateFormat()
		{
			return "yyyy-MM-ddTHH:mm:ss";
		}

		public XMLImporter(string path) : base(path) { }

		public override FileParsingResult ParseTransactionFile()
		{

			var results = new FileParsingResult();

			try
			{
				XmlDocument xDoc = new XmlDocument();
				xDoc.Load(FilePath);
				XmlElement xRoot = xDoc.DocumentElement;

				var transactions = xRoot.SelectNodes("/Transactions/Transaction");

				foreach (XmlNode xn in transactions)
				{
					var rawData = new TransactionRawData();

					// Get Transaction ID
					if (xn.Attributes.Count > 0)
					{
						XmlNode attr = xn.Attributes.GetNamedItem("id");
						if (attr != null)
							rawData.TransactionId = attr.Value;
					}

					foreach (XmlNode childnode in xn.ChildNodes)
					{
						if (childnode.Name == "TransactionDate")
						{
							rawData.TransactionDate = StringHelper.StripQuotes(childnode.InnerText);
						}
						if (childnode.Name == "Status")
						{
							rawData.Status = StringHelper.StripQuotes(childnode.InnerText);
						}

						if (childnode.Name == "PaymentDetails")
						{
							foreach (XmlNode innerNode in childnode.ChildNodes)
							{
								if (innerNode.Name == "Amount")
								{
									rawData.Amount = StringHelper.StripQuotes(innerNode.InnerText);
								}
								if (innerNode.Name == "CurrencyCode")
								{
									rawData.CurrencyCode = StringHelper.StripQuotes(innerNode.InnerText);
								}
							}
						}
					}

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
			catch (Exception e)
			{
				results.InvalidItems.Add(e.Message);
			}

			return results;

		}
	}
}
