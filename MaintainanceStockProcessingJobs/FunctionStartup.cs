using MaintainanceStockProcessingJobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stocks_Data_Processing.Interfaces.Jobs;
using Stocks_Data_Processing.Interfaces.Services;
using Stocks_Data_Processing.Jobs;
using Stocks_Data_Processing.Services;
using StocksFinalSolution.BusinessLogic.Interfaces.Services;
using StocksProccesing.Relational;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Extension_Methods.DI;
using System;
using StocksFinalSolution.BusinessLogic.Features.StocksMarketMetricsCalculator;
using StocksFinalSolution.BusinessLogic.Features.StocksMarketSummaryGenerator;

[assembly: FunctionsStartup(typeof(FunctionStartup))]

namespace MaintainanceStockProcessingJobs
{
    public class FunctionStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string databaseConnectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            Console.WriteLine(databaseConnectionUrl);

            builder.Services.AddDbContext<StocksMarketContext>(options =>
            {
                options.UseSqlServer(databaseConnectionUrl ?? DatabaseSettings.ConnectionString);
            });

            builder.Services.AddPersistence();

            builder.Services
                .AddTransient<IPricesDisparitySimulator, PricesDisparitySimulator>()
                .AddTransient<IStocksSummaryGenerator, StocksSummaryGenerator>()
                .AddTransient<IStockMarketDisplayPriceCalculator, StockMarketDisplayPriceCalculator>()
                .AddTransient<IStockMarketOrderTaxesCalculator, StockMarketOrderTaxesCalculator>()
                .AddTransient<IStockMarketProfitCalculator, StockMarketProfitCalculator>()
                .AddTransient<IStocksTrendCalculator, StocksTrendCalculator>()
                .AddTransient<ITransactionSummaryCalculator, TransactionSummaryCalculator>()

                .AddScoped<IStockMarketProfitCalculator, StockMarketProfitCalculator>()
                .AddScoped<IStocksSummaryGenerator, StocksSummaryGenerator>()
                .AddScoped<IMaintainCurrentStockData, MaintainCurrentStockData>()
                .AddScoped<IMaintainPredictionsUpToDate, MaintainPredictionsUpToDate>()
                .AddScoped<IMaintainTaxesCollected, MaintainTaxesCollected>()
                .AddScoped<IMaintainTransactionsUpdated, MaintainTransactionsUpdated>()
                .AddScoped<IMaintainPeriodicalSummaries, MaintainPeriodicalSummaries>()
                .AddScoped<ICurrentStockInfoDataScraperService, CurrentStockInfoDataScraperService>()
                .AddScoped<ICurrentStockInfoYahooScraperService, CurrentStockInfoYahooScraperService>()
                .AddScoped<ICurrentStockInfoGoogleScraperService, CurrentStockInfoGoogleScraperService>()
                .AddScoped<IScraperService, ScraperService>();
        }
    }
}
