using Autofac;
using Microsoft.Extensions.Logging;

namespace Stocks_Data_Processing
{
    class Program
    {
        static void Main()
        {
            var DIContainer = DIContainerConfig.Configure();

            using var scope = DIContainer.BeginLifetimeScope();
            var application = scope.Resolve<IApplication>();

            application.Run();
        }
    }
}
