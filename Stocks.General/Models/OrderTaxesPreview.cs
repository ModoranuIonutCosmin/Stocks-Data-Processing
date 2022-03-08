namespace Stocks.General.Models
{
    public class OrderTaxesPreview
    {
        public decimal CurrentPrice { get; set; }
        public decimal InvestedAmount { get; set; }
        public decimal Trend { get; set; }
        public decimal TodayIncrement { get; set; }
        public decimal WeekdayTax { get; set; }
        public decimal WeekendTax { get; set; }
        public decimal PercentageExposed { get; set; }
        public decimal UnitsPaid { get; set; }
        public decimal ExtraMoneyNeeded { get; set; } = 0;
    }
}
