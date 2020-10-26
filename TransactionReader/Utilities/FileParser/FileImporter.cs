using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionReader.Utilities.FileParser
{
	public abstract class FileImporter
	{
		protected string FilePath { get; set; }

		public FileImporter(string path)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("Path is null");
			if (!File.Exists(path)) throw new FileNotFoundException("Check path " + path);

			FilePath = path;
		}

		protected abstract string DateFormat();

		public abstract FileParsingResult ParseTransactionFile();

		public static FileImporter Create(string filePath, string fileExtension)
		{
			switch (fileExtension)
			{
				case ".csv": return new CSVImporter(filePath);
				case ".xml": return new XMLImporter(filePath);
			}
			return null;
		}
	}
}
