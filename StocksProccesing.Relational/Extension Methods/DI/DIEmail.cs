using Microsoft.Extensions.DependencyInjection;
using StocksFinalSolution.BusinessLogic.Interfaces.Email;
using StocksProccesing.Relational.Email;

namespace StocksProccesing.Relational.Extension_Methods.DI;

public static class DIEmail
{
    public static void AddEmailServices(this IServiceCollection builder)
    {
        builder.AddTransient<IDirectEmailSender, DirectEmailSender>()
            .AddTransient<ITemplatedEmailSender, TemplatedEmailSender>()
            .AddTransient<IGeneralPurposeEmailService, GeneralPurposeEmailService>();
    }
}