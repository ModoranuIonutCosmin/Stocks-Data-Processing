namespace StocksProcessing.API.Models
{
    public class MarketOrder
    {
        public string Token { get; set; }
        public bool IsBuy { get; set; }
        public string Ticker { get; set; }

        public int Leverage { get; set; }

        public double StopLossAmount { get; set; }

        public double TakeProfitAmount { get; set; }

        public double InvestedAmount { get; set; }
    }
}
