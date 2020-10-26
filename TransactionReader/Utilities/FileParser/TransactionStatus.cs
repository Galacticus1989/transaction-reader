using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionReader.Utilities.FileParser
{
	public class TransactionStatus
	{
		public enum Status
		{
			Approved,
			Rejected,
			Failed,
			Done,
			Finished
		}

		public enum StatusCode
		{
			A,
			R,
			D
		}

		/// <summary>
		/// Status codes mapping
		/// </summary>
		private static readonly Dictionary<string, string> statusMapping = new Dictionary<string, string>
		{
			{Status.Approved.ToString(), StatusCode.A.ToString()},
			{Status.Failed.ToString(), StatusCode.R.ToString()},
			{Status.Rejected.ToString(), StatusCode.R.ToString()},
			{Status.Finished.ToString(), StatusCode.D.ToString()},
			{Status.Done.ToString(), StatusCode.D.ToString()},
		};

		/// <summary>
		/// Access the Status codes mapping
		/// </summary>
		public static string GetStatusCode(string status)
		{
			// Try to get the Status code in the Status mapping
			string result;
			if (statusMapping.TryGetValue(status, out result))
			{
				return result;
			}
			else
			{
				return string.Empty;
			}
		}

	}
}
