using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.Model;
using System.Diagnostics.CodeAnalysis;

namespace StocksProccesing.Relational.DataAccess
{
    public class StocksMarketContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<StocksPriceData> PricesData { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<StocksTransaction> Transactions { get; set; }
        public DbSet<MaintenanceAction> Actions { get; set; }
        public DbSet<StocksOHLC> Summaries { get; set; }

        public DbSet<Order> Orders { get; set; }

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
            //modelBuilder.Entity<StocksDailySummaryModel>
            //    ().HasNoKey().ToView(null);

            //Indexi
            modelBuilder.Entity<Company>()
            .HasIndex(d => d.Ticker)
            .IsUnique();

            modelBuilder.Entity<StocksPriceData>()
            .HasIndex(p => p.CompanyTicker);

            modelBuilder.Entity<StocksPriceData>()
                .HasIndex(p => p.Date);

            modelBuilder.Entity<StocksTransaction>().HasIndex(p => p.UniqueActionStamp)
                .IsUnique();

            modelBuilder.Entity<StocksOHLC>()
                .HasAlternateKey(c => new { c.Date, c.Period, c.CompanyTicker });
        }

    }
}
