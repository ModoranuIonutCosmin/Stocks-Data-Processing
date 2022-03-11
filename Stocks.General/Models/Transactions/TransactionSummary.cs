namespace Stocks.General.Models.Transactions
{
    public class TransactionSummary
    {
        public string Name { get; set; }
        public string UrlLogo { get; set; }
        public string Description { get; set; }
        public string Ticker { get; set; }
        public decimal AverageInitial { get; set; }
        public decimal TotalInvested { get; set; }
        public decimal TotalPl { get; set; }
        public decimal TotalPlPercentage { get; set; }
        public decimal TotalUnits { get; set; }
        public decimal Value { get; set; }
    }
}