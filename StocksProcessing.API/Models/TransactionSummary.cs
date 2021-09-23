using Stocks.General.ExtensionMethods;

namespace StocksProcessing.API.Models
{
    public class TransactionSummary
    {
        public string Ticker { get; set; }

        public bool IsBuy { get; set; }

        public double UnitsPurchased { get; set; }

        public double InitialPrice { get; set; }

        public double CurrentSellPrice { get; set; }
        public double InvestedAmount { get; set; }

        public double CurrentPrice {
            get => IsBuy ? CurrentSellPrice : CurrentBuyPrice;
        }

        public double CurrentBuyPrice { 
            get
            {
                return StockMarketCalculus.CalculateBuyPrice(CurrentSellPrice);
            } 
        }


        public double ProfitOrLoss {

            get
            {
                return ((CurrentPrice - InitialPrice) * UnitsPurchased * (!IsBuy ? -1 : 1)).TruncateToDecimalPlaces(3);
            } 
        }

        public double ProfitOrLossPercentage
        {
            get
            {
                return (ProfitOrLoss / (InitialPrice * UnitsPurchased)).TruncateToDecimalPlaces(3);
            }
        }
    }
}
