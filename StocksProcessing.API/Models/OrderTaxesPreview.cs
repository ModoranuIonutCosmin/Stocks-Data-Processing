namespace StocksProcessing.API.Models
{
    public class OrderTaxesPreview
    {
        public double CurrentPrice { get; set; }
        public double Trend { get; set; }

        public double TodayIncrement { get; set; }
        public double WeekdayTax { get; set; }
        public double WeekendTax { get; set; }
        public double PercentageExposed { get; set; }
        public double UnitsPaid { get; set; }
    }
}
