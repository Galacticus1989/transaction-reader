using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionReader.Models;

namespace TransactionReader.Data
{
    public class TransationContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }

        public TransationContext() { }

        public TransationContext(DbContextOptions<TransationContext> options)
            : base(options) { }
    }
}
