using Microsoft.Extensions.DependencyInjection;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.DataAccess.V1;

namespace StocksProccesing.Relational.Extension_Methods.DI;

public static class DIPersistence
{
    public static void AddPersistence(this IServiceCollection builder)
    {
        builder
            .AddTransient<IUsersRepository, UsersRepository>()
            .AddTransient<IOrdersRepository, OrdersRepository>()
            .AddTransient<ITransactionsRepository, TransactionsRepository>()
            .AddTransient<IStockPricesRepository, StockPricesRepository>()
            .AddTransient<ICompaniesRepository, CompaniesRepository>()
            .AddTransient<IStockSummariesRepository, StockSummariesRepository>();
    }
}