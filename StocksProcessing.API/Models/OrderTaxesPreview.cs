using Stocks.General.ConstantsConfig;

namespace StocksProcessing.API.Models
{
    public class OrderTaxesPreview
    {
        public double CurrentPrice { get; set; }
        public double InvestedAmount { get; set; }
        public double Trend { get; set; }
        public double TodayIncrement { get; set; }
        public double WeekdayTax { get; set; }
        public double WeekendTax { get => WeekdayTax * TaxesConfig.WeekendOvernightMultiplier;} 
        public double PercentageExposed { get; set; }
        public double UnitsPaid { get; set; }
        public double ExtraMoneyNeeded { get; set; } = 0;
        public double StopLossMax 
        { 
            get 
            {
                return InvestedAmount / 2; 
            } 
        }
    }
}
