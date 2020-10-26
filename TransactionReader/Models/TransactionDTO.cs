using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionReader.Models
{
    public class TransactionDTO
    {
        public string id { get; set; }
        public string payment { get; set; }
        public string status { get; set; }
    }
}
