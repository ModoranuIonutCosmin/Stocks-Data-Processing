using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stocks_Data_Processing;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.DataAccess.V1;

namespace StockBulkGatherer;

public static class DIContainerConfig
{
    public static ILifetimeScope Scope { get; set; }

    private static void ConfigureLogging(ILoggingBuilder log)
    {
        log.ClearProviders();
        log.AddConsole();
        log.SetMinimumLevel(LogLevel.Warning);
    }

    public static IContainer Configure()
    {
        var builder = new ContainerBuilder();

        ServiceCollection serviceCollection = new();

        serviceCollection.AddDbContext<StocksMarketContext>(opt =>
            opt.UseSqlServer(DatabaseSettings.ConnectionString));

        builder.Populate(serviceCollection);

        builder.RegisterType<Application>().As<IApplication>();

        builder.Register(handler => LoggerFactory.Create(ConfigureLogging))
            .As<ILoggerFactory>()
            .SingleInstance()
            .AutoActivate();

        builder.RegisterGeneric(typeof(Logger<>))
            .As(typeof(ILogger<>))
            .SingleInstance();

        builder.RegisterType<StockContextFactory>()
            .SingleInstance();

        builder.Register(c => new HttpClient())
            .As<HttpClient>()
            .SingleInstance();
        
        //repositories
        builder.RegisterType<CompaniesRepository>().As<ICompaniesRepository>();
        builder.RegisterType<StockPricesRepository>().As<IStockPricesRepository>();

        //stocks data gather service
        builder.RegisterType<AlphaVantageStocksPricesGatherer>().As<IApiStockPricesGatherer>();


        return builder.Build();
    }

    public static T Resolve<T>()
    {
        if (Scope is null)
            return default;

        return Scope.Resolve<T>();
    }
}