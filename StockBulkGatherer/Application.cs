using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stocks.General;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;

namespace StockBulkGatherer;

public class Application : IApplication
{
    private static readonly IReadOnlyList<string> WatchList
        = Enum.GetValues(typeof(StocksTicker)).Cast<StocksTicker>()
            .Select(s => s.ToString()).ToList();

    private static ICompaniesRepository _companiesRepository;
    private readonly StockContextFactory _dbContextFactory;
    private readonly IApiStockPricesGatherer _pricesGatherer;

    public Application(IApiStockPricesGatherer pricesGatherer, 
        ICompaniesRepository companiesRepository,
        StockContextFactory dbContextFactory)
    {
        _pricesGatherer = pricesGatherer;
        _companiesRepository = companiesRepository;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<StocksPriceData>> GatherAllCompaniesData()
    {
        var alphaVantageContext = new AlphaVantageContext();
        var limitPerMinute = alphaVantageContext.Limits.CallsPerMinute;
        
        var to = DateTimeOffset.UtcNow;
        var from = DateTimeOffset.UtcNow.AddMonths(-3);

        var pricesData = new List<StocksPriceData>();

        _companiesRepository.EnsureCompaniesDataExists();

        foreach (var ticker in WatchList)
        {
            if (limitPerMinute < 0)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                limitPerMinute = alphaVantageContext.Limits.CallsPerMinute;
            }

            var prices = await _pricesGatherer.Gather(ticker, from, to, 50000);
            pricesData.AddRange(prices.results.InterpolateMissingValues());
            
            limitPerMinute -= prices.callsMade;
        }

        return pricesData;
    }

    public async Task Run()
    {
        
        var pricesChunks = (await GatherAllCompaniesData())
            .GroupBy(g => g.CompanyTicker)
            .Select(group => group.ToList());

        var insertTasks = new List<Task>();

        foreach (var chunk in pricesChunks)
        {
            insertTasks.Add(Task.Run(async () =>
            {
                var dbContext = _dbContextFactory.Create();
                
                Console.WriteLine("Adaugam");

                await dbContext.PricesData.AddRangeAsync(chunk);

                await dbContext.SaveChangesAsync();
            }));
        }

        await Task.WhenAll(insertTasks);
    }
}