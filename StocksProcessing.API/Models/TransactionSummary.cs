using System;

namespace StocksProcessing.API.Models
{
    public class TransactionSummary
    {
        public string Ticker { get; set; }

        public bool IsBuy { get; set; }

        public double UnitsPurchased { get; set; }

        public double InitialPrice { get; set; }

        public double CurrentPrice { get; set; }

        public double InvestedAmount { get => UnitsPurchased * InitialPrice; }

        public double ProfitOrLoss {

            get
            {
                return Math.Round((CurrentPrice - InitialPrice) * UnitsPurchased, 3) * (!IsBuy ? -1 : 1);
            } 
        }

        public double ProfitOrLossPercentage
        {
            get
            {
                return Math.Round(ProfitOrLoss / (InitialPrice * UnitsPurchased), 2);
            }
        }
    }
}
