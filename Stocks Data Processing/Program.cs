﻿using Autofac;
using System.Threading.Tasks;

namespace Stocks_Data_Processing
{
    static class Program
    {
        static async Task Main()
        {
            var DIContainer = DIContainerConfig.Configure();

            using var scope = DIContainer.BeginLifetimeScope();
            DIContainerConfig.Scope = scope;

            await scope.Resolve<IApplication>().Run();
        }
    }
}
