using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server_TSD.Models.db_DaxBko
{
    public class db_DaxBkoContext : DbContext
    {
        public DbSet<SalesTable> SalesTable { get; set; }
        public DbSet<SalesLine> SalesLine { get; set; }
        public DbSet<InventDim> InventDim { get; set; }

        public DbSet<ProdLabelsNum> ProdLabelsNum { get; set; }

        public DbSet<WrkCtrTable> WrkCtrTable { get; set; }
        public DbSet<ProdShiftTable_UN> ProdShiftTable_UN { get; set; }

        public db_DaxBkoContext(DbContextOptions<db_DaxBkoContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SalesTable>()
                .HasOne(b => b.salesLines)
                .WithOne(i => i.salesTables)
                .HasForeignKey<SalesLine>(b => b.SalesId);
        }
    }
}
