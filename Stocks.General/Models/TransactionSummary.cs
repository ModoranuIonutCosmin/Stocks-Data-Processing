namespace Stocks.General.Models
{
    public class TransactionSummary
    {
        public string Ticker { get; set; }

        public bool IsBuy { get; set; }

        public decimal UnitsPurchased { get; set; }

        public decimal InitialPrice { get; set; }

        public decimal CurrentSellPrice { get; set; }
        public decimal InvestedAmount { get; set; }

        public decimal CurrentPrice
        {
            get; set;
        }

        public decimal CurrentBuyPrice { get; set; }


        public decimal ProfitOrLoss
        {

            get; set;
        }

        public decimal ProfitOrLossPercentage
        {
            get; set;
        }


        //public int Id { get; set; }

        //public bool IsBuy { get; set; }

        //public decimal UnitsPurchased { get; set; }

        //public decimal InitialPrice { get; set; }

        //public decimal CurrentPrice { get; set; }

        //public decimal InvestedAmount { get; set; }

        //public decimal StopLossAmount { get; set; }

        //public decimal TakeProfitAmount { get; set; }

        //public double Leverage { get; set; }

        //public DateTimeOffset Date { get; set; }

        //public double Value { get => InitialPrice * UnitsPurchased + ProfitOrLoss; }

        //public bool IsCFD
        //{
        //    get
        //    {
        //        return Leverage > 1 || !IsBuy;
        //    }
        //}
        //public decimal ProfitOrLoss
        //{
        //    get
        //    {
        //        return ((CurrentPrice - InitialPrice) * UnitsPurchased * (!IsBuy ? -1 : 1)).TruncateToDecimalPlaces(3);
        //    }
        //}

        //public double ProfitOrLossPercentage
        //{
        //    get
        //    {
        //        return (ProfitOrLoss / (InitialPrice * UnitsPurchased)).TruncateToDecimalPlaces(3);
        //    }
        //}
    }
}
