using Autofac;
using Newtonsoft.Json;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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
