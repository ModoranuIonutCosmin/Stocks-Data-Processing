namespace Stocks.General.Models.StocksInfoAggregates;

public class PlaceMarketOrderRequest
{
    public string Token { get; set; }
    public bool IsBuy { get; set; }
    public string Ticker { get; set; }

    public int Leverage { get; set; }

    public decimal StopLossAmount { get; set; }

    public decimal TakeProfitAmount { get; set; }

    public decimal InvestedAmount { get; set; }
}