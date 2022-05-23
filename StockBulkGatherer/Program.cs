using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Stocks.General;
using Stocks_Data_Processing;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.DataAccess;

namespace StockBulkGatherer;

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