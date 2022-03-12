using System.Threading.Tasks;
using Stocks.General.Models;
using Stocks.General.Models.Funds;
using Stocks.General.Models.StocksInfoAggregates;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.Portofolio;

public interface IPortofolioService
{
    Task<BalanceRefillOrder> ReplenishBalance(ApplicationUser userRequesting,
        PaymentDetails paymentDetails);

    Task<OrderTaxesPreview> GetTransactionTaxes(ApplicationUser requestingUser,
        string ticker, decimal invested, decimal leverage, bool isBuy);

    Task<PlaceMarketOrderRequest> PlaceOrder(ApplicationUser requestingUser,
        PlaceMarketOrderRequest marketOrder);

    TradingContext GetTradingContext(ApplicationUser userRequesting);
}