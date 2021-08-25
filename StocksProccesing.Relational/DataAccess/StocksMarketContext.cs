using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.Model;
using System.Diagnostics.CodeAnalysis;

namespace StocksProccesing.Relational.DataAccess
{
    public class StocksMarketContext : DbContext
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
    }
}
