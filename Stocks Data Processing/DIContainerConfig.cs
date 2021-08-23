using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StocksProccesing.Relational.DataAccess;
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
            log.SetMinimumLevel(LogLevel.Information);
            log.AddConsole();
        }
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();


            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<StocksDataHandlingLogic>().As<IStocksDataHandlingLogic>();

            IServiceCollection databaseServiceCollection = new ServiceCollection();

            databaseServiceCollection.AddDbContext<StocksMarketContext>();

            builder.Populate(databaseServiceCollection);

            builder.Register(handler => LoggerFactory.Create(ConfigureLogging))
                .As<ILoggerFactory>()
                .SingleInstance()
                .AutoActivate();


            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();

            builder.Register(c => new HttpClient())
                   .As<HttpClient>()
                   .SingleInstance();

            builder.RegisterAssemblyTypes(Assembly.Load(nameof(Stocks_Data_Processing).Replace('_', ' ')))
                .Where(t => t.Namespace.Contains("Utilities"))
                .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name));


            return builder.Build();
        }
    }
}
