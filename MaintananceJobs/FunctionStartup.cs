using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Stocks_Data_Processing.Interfaces.Jobs;
using Stocks_Data_Processing.Interfaces.Services;
using Stocks_Data_Processing.Jobs;
using Stocks_Data_Processing.Services;
using StocksProccesing.Relational.Extension_Methods.DI;

[assembly: FunctionsStartup(typeof(MaintananceJobs.FunctionStartup))]

namespace MaintananceJobs
{
    public class FunctionStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddPersistence();

            builder.Services
                .AddScoped<IMaintainCurrentStockData, MaintainCurrentStockData>()
                .AddScoped<IMaintainPredictionsUpToDate, MaintainPredictionsUpToDate>()
                .AddScoped<IMaintainTaxesCollected, MaintainTaxesCollected>()
                .AddScoped<IMaintainTransactionsUpdated, MaintainTransactionsUpdated>()
                .AddScoped<IMaintainPeriodicalSummaries, MaintainPeriodicalSummaries>()
                .AddScoped<ICurrentStockInfoDataScraperService, CurrentStockInfoDataScraperService>()
                .AddScoped<ICurrentStockInfoYahooScraperService, CurrentStockInfoYahooScraperService>()
                .AddScoped<ICurrentStockInfoGoogleScraperService, CurrentStockInfoGoogleScraperService>()
                .AddScoped<IScraperService, ScraperService>();

            //      builder.RegisterType<MaintainPredictionsUpToDate>().As<IMaintainPredictionsUpToDate>();
            //builder.RegisterType<MaintainCurrentStockData>().As<IMaintainCurrentStockData>();
            //builder.RegisterType<MaintainTaxesCollected>().As<IMaintainTaxesCollected>();
            //builder.RegisterType<MaintainTransactionsUpdated>().As<IMaintainTransactionsUpdated>();
            //builder.RegisterType<MaintainPeriodicalSummaries>().As<IMaintainPeriodicalSummaries>();

            //builder.RegisterType<CurrentStockInfoDataScraperService>().As<ICurrentStockInfoDataScraperService>();
            //builder.RegisterType<CurrentStockInfoYahooScraperService>().As<ICurrentStockInfoYahooScraperService>();
            //builder.RegisterType<CurrentStockInfoGoogleScraperService>().As<ICurrentStockInfoGoogleScraperService>();
            //builder.RegisterType<ScraperService>().As<IScraperService>();
        }
    }
}
