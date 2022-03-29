using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace StocksProccesing.Relational.DataAccess;

public class StockContextFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public StockContextFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public StocksMarketContext Create()
    {
        var options = new DbContextOptionsBuilder<StocksMarketContext>()
            .UseSqlServer(DatabaseSettings.ConnectionString)
            .UseLoggerFactory(_loggerFactory)
            .Options;

        return new StocksMarketContext(options);
    }
}