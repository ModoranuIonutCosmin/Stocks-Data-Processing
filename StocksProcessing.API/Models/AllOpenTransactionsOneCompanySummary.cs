namespace StocksProcessing.API.Models
{
    public class AllOpenTransactionsOneCompanySummary
    {
        public AllOpenTransactionsOneCompanySummary()
        {
        }

        public string Ticker { get; set; }
        public double AverageInitial { get; set; }
        public double TotalInvested { get; set; }
        public double TotalPl { get; set; }
        public double TotalPlPercentage { get; set; }
        public double TotalUnits { get; set; }
    }
}