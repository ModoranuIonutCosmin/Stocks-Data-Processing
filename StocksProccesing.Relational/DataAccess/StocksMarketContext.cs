using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StocksProccesing.Relational.Encryption;
using StocksProccesing.Relational.Model;

namespace StocksProccesing.Relational.DataAccess;

public class StocksMarketContext : IdentityDbContext<ApplicationUser>,
    IDataProtectionKeyContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    public DbSet<StocksPriceData> PricesData { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<StocksTransaction> Transactions { get; set; }
    public DbSet<MaintenanceAction> Actions { get; set; }
    public DbSet<StocksOhlc> Summaries { get; set; }
    public DbSet<Order> Orders { get; set; }

    public StocksMarketContext()
    {
    }

    public StocksMarketContext([NotNull] DbContextOptions options)
        : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //Criptarea datelor senzitive short term - fara querying

        // builder.Entity<ApplicationUser>(entityBuilder =>
        // {
        //     this.AddDataProtectionConverters(entityBuilder);
        // });
        
        //Campuri indetity
        
        builder.Entity<ApplicationUser>().Property(u => u.UserName)
            .HasMaxLength(256);
        
        builder.Entity<ApplicationUser>().Property(u => u.NormalizedUserName)
            .HasMaxLength(256);
        
        builder.Entity<ApplicationUser>().Property(u => u.Email)
            .HasMaxLength(256);
        
        builder.Entity<ApplicationUser>().Property(u => u.NormalizedEmail)
            .HasMaxLength(256);
        
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