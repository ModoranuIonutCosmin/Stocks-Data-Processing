using Autofac;
using System;
using System.Threading.Tasks;
using Stocks.General.ExtensionMethods;

namespace Stocks_Data_Processing
{
    class Program
    {
        static async Task Main()
        {
            var DIContainer = DIContainerConfig.Configure();

            using var scope = DIContainer.BeginLifetimeScope();
            var application = scope.Resolve<IApplication>();

            await application.Run();
        }
    }
}
