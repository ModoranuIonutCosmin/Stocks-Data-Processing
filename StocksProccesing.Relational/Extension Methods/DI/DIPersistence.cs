using Microsoft.Extensions.DependencyInjection;
using StocksFinalSolution.BusinessLogic.Interfaces.Repositories;
using StocksProccesing.Relational.DataAccess.V1;

namespace StocksProccesing.Relational.Extension_Methods.DI;

public static class DIPersistence
{
    public static void AddPersistence(this IServiceCollection builder)
    {
        builder
            .AddScoped<IUsersRepository, UsersRepository>()
            .AddScoped<IOrdersRepository, OrdersRepository>()
            .AddScoped<ITransactionsRepository, TransactionsRepository>()
            .AddScoped<IStockPricesRepository, StockPricesRepository>()
            .AddScoped<ICompaniesRepository, CompaniesRepository>()
            .AddScoped<IStockSummariesRepository, StockSummariesRepository>()
            .AddScoped<ISubscriptionsRepository, SubscriptionsRepository>()
            .AddScoped<IMaintainanceJobsRepository, MaintainanceJobsRepository>();

    }

}