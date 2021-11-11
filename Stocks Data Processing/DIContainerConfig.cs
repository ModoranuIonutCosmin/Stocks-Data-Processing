using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz.Impl;
using Stocks_Data_Processing.Actions;
using Stocks_Data_Processing.QuartzDI;
using Stocks_Data_Processing.Utilities;
using StocksFinalSolution.BusinessLogic.StocksMarketMetricsCalculator;
using StocksFinalSolution.BusinessLogic.StocksMarketSummaryGenerator;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.DataAccess.V1.Repositories;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Stocks_Data_Processing
{
    public static class DIContainerConfig
    {
        public static ILifetimeScope Scope { get; set; }
        private static void ConfigureLogging(ILoggingBuilder log)
        {
            log.ClearProviders();
            log.SetMinimumLevel(LogLevel.Warning);
            log.AddConsole();
        }

        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<StocksMarketContext>();

            builder.Populate(serviceCollection);

            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<StocksDataHandlingLogic>().As<IStocksDataHandlingLogic>();

            builder.Register(handler => LoggerFactory.Create(ConfigureLogging))
                .As<ILoggerFactory>()
                .SingleInstance()
                .AutoActivate();

            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();

            builder.RegisterType<StockContextFactory>()
                   .SingleInstance();

            builder.RegisterType<StdSchedulerFactory>()
                .SingleInstance()
                .AutoActivate();

            builder.Register(c => new HttpClient())
                   .As<HttpClient>()
                   .SingleInstance();

            /// Repositories, calculators, schedulers
            /// 


            builder.RegisterType<MaintainanceTasksScheduler>().As<IMaintainanceTasksScheduler>();
            builder.RegisterType<StocksSummaryGenerator>().As<IStocksSummaryGenerator>();
            builder.RegisterType<StocksSummaryGenerator>().As<IStocksSummaryGenerator>();
            builder.RegisterType<StocksTrendCalculator>().As<IStocksTrendCalculator>();



            builder.RegisterType<UsersRepository>().As<IUsersRepository>();
            builder.RegisterType<OrdersRepository>().As<IOrdersRepository>();
            builder.RegisterType<StockSummariesRepository>().As<IStockSummariesRepository>();
            builder.RegisterType<MaintainanceJobsRepository>().As<IMaintainanceJobsRepository>();
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

        public static T Resolve<T>()
        {
            if (Scope is null)
                return default;

            return Scope.Resolve<T>();
        }
    }
}
