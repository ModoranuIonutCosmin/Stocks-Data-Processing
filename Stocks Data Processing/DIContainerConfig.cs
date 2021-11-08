using Autofac;
using Autofac.Extras.Quartz;
using Microsoft.Extensions.Logging;
using Quartz.Impl;
using Stocks_Data_Processing.QuartzDI;
using Stocks_Data_Processing.Utilities;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Repositories;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Stocks_Data_Processing
{
    public static class DIContainerConfig
    {

        private static void ConfigureLogging(ILoggingBuilder log)
        {
            log.ClearProviders();
            log.SetMinimumLevel(LogLevel.Warning);
            log.AddConsole();
        }
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();


            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<StocksDataHandlingLogic>().As<IStocksDataHandlingLogic>();

            builder.RegisterType<StockContextFactory>().SingleInstance();
            builder.RegisterType<StocksMarketContext>().SingleInstance();

            builder.Register(handler => LoggerFactory.Create(ConfigureLogging))
                .As<ILoggerFactory>()
                .SingleInstance()
                .AutoActivate();

            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();

            builder.RegisterType<StdSchedulerFactory>()
                .SingleInstance()
                .AutoActivate();

            builder.Register(c => new HttpClient())
                   .As<HttpClient>()
                   .SingleInstance();

            /// Repositories, calculators
            /// 


            builder.RegisterType<UsersRepository>().As<IUsersRepository>();
            builder.RegisterType<OrdersRepository>().As<IOrdersRepository>();
            builder.RegisterType<TransactionsRepository>().As<ITransactionsRepository>();
            builder.RegisterType<StockPricesRepository>().As<IStockPricesRepository>();
            builder.RegisterType<CompaniesRepository>().As<ICompaniesRepository>();
            builder.RegisterType<StockMarketDisplayPriceCalculator>().As<IStockMarketDisplayPriceCalculator>();
            builder.RegisterType<StockMarketOrderTaxesCalculator>().As<IStockMarketOrderTaxesCalculator>();
            builder.RegisterType<PricesDisparitySimulator>().As<IPricesDisparitySimulator>();
            builder.RegisterType<StockMarketProfitCalculator>().As<IStockMarketProfitCalculator>();
            builder.RegisterType<TransactionSummaryCalculator>().As<ITransactionSummaryCalculator>();

            ///Quartz net

            builder.Register(_ => new ScopedDependency("global"))
            .AsImplementedInterfaces()
            .SingleInstance();

            builder.RegisterModule(new QuartzAutofacFactoryModule
            {
                JobScopeConfigurator = (builder, jobScopeTag) =>
                {
                    // override dependency for job scope
                    builder.Register(_ => new ScopedDependency("job-local " + DateTime.UtcNow.ToLongTimeString()))
                        .AsImplementedInterfaces()
                        .InstancePerMatchingLifetimeScope(jobScopeTag);
                },
            });

            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(MaintainPredictionsUpToDate).Assembly));
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(MaintainCurrentStockData).Assembly));
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(MaintainTaxesCollected).Assembly));
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(MaintainTransactionsUpdated).Assembly));
            ///

            builder.RegisterAssemblyTypes(Assembly.Load(nameof(Stocks_Data_Processing).Replace('_', ' ')))
                .Where(t => t.Namespace.Contains("Utilities"))
                .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name));

            return builder.Build();
        }
    }
}
