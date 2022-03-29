using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.DataAccess;

public class StocksMarketContext : IdentityDbContext<ApplicationUser>
{
    public StocksMarketContext()
    {
    }

    public StocksMarketContext([NotNull] DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<StocksPriceData> PricesData { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<StocksTransaction> Transactions { get; set; }
    public DbSet<MaintenanceAction> Actions { get; set; }
    public DbSet<StocksOhlc> Summaries { get; set; }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //Instante doar ca rezultat fara tabel - rezultate din stored procedures

        //Indexi
        builder.Entity<Company>()
            .HasIndex(d => d.Ticker)
            .IsUnique();

        builder.Entity<StocksPriceData>()
            .HasIndex(p => p.CompanyTicker);

        builder.Entity<StocksPriceData>()
            .HasIndex(p => p.Date);

        builder.Entity<StocksTransaction>().HasIndex(p => p.UniqueActionStamp)
            .IsUnique();

        builder.Entity<MaintenanceAction>().HasIndex(p => p.Name)
            .IsUnique();
        builder.Entity<StocksOhlc>().HasIndex(p => p.Period);
    }
}