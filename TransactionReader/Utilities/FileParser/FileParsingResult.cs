using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionReader.Models;

namespace TransactionReader.Utilities.FileParser
{
	public class FileParsingResult
	{
		public Dictionary<string, Transaction> Results { get; set; }
		public List<string> InvalidItems { get; set; }

		// Treat file as invalid if any error...
		public bool Failed => InvalidItems.Any();
		public bool Success => !InvalidItems.Any();

		public FileParsingResult()
		{
			Results = new Dictionary<string, Transaction>();
			InvalidItems = new List<string>();
		}
	}
}
