using System.Threading.Tasks;
using Autofac;

namespace Stocks_Data_Processing;

internal static class Program
{
    private static async Task Main()
    {
        var DIContainer = DIContainerConfig.Configure();

        using var scope = DIContainer.BeginLifetimeScope();
        DIContainerConfig.Scope = scope;
        
        await scope.Resolve<IApplication>().Run();
    }
}