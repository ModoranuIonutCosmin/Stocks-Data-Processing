using Autofac;
using System.Threading.Tasks;

namespace Stocks_Data_Processing
{
    class Program
    {
        static async Task Main()
        {
            var DIContainer = DIContainerConfig.Configure();

            using var scope = DIContainer.BeginLifetimeScope();
            DIContainerConfig.Scope = scope;
            var application = scope.Resolve<IApplication>();


            await application.Run();
        }
    }
}
