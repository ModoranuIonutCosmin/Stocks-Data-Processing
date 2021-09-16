using System;
using Stocks.General.ExtensionMethods;

namespace StocksProcessing.API.Models
{
    public class TransactionFullInfo
    {
        public string Ticker { get; set; }

        public bool IsBuy { get; set; }

        public double UnitsPurchased { get; set; }

        public double InitialPrice { get; set; }

        public double CurrentPrice { get; set; }

        public double InvestedAmount { get; set; }

        public double StopLossAmount { get; set; }

        public double TakeProfitAmount { get; set; }

        public double Leverage { get; set; }

        public DateTimeOffset Date { get; set; }

        public bool IsCFD { 
            get
            {
                return Leverage > 1 || !IsBuy;
            }
        }
        public double ProfitOrLoss
        {
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
