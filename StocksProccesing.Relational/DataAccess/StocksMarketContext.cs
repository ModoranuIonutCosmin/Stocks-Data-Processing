using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.Model;
using StocksProcessing.API.Models;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace StocksProccesing.Relational.DataAccess
{
    public class StocksMarketContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<StocksPriceData> PricesData { get; set; }
        public DbSet<Company> Companies { get; set; }

        public StocksMarketContext()
        {

        }

        public StocksMarketContext([NotNull] DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(DatabaseSettings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Instante doar ca rezultat fara tabel - rezultate din stored procedures
            modelBuilder.Entity<StocksDailySummaryModel>
                ().HasNoKey().ToView(null);

            //Indexi
            modelBuilder.Entity<Company>()
            .HasIndex(d => d.Ticker)
            .IsUnique();

            modelBuilder.Entity<StocksPriceData>()
            .HasIndex(p => p.Date);

            modelBuilder.Entity<StocksPriceData>()
            .HasIndex(p => p.CompanyTicker);

            modelBuilder.Entity<StocksPriceData>()
            .HasIndex(p => p.Prediction);
        }

        protected static void OnStateChange(object sender, StateChangeEventArgs args)
        {
            if (args.OriginalState == ConnectionState.Closed
                && args.CurrentState == ConnectionState.Open)
            {
                using (DbCommand _Command = ((DbConnection)sender).CreateCommand())
                {
                    _Command.CommandType = CommandType.Text;
                    _Command.CommandText = "SET ARITHABORT ON;";
                    _Command.ExecuteNonQuery();
                }
            }
        }
    }
}
