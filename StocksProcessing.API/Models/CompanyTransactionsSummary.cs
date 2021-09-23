namespace StocksProcessing.API.Models
{
    public class CompanyTransactionsSummary
    {
        public CompanyTransactionsSummary()
        {
        }

        public string Name {  get; set; }
        public string UrlLogo {  get; set; }
        public string Description {  get; set; }
        public string Ticker { get; set; }
        public double AverageInitial { get; set; }
        public double TotalInvested { get; set; }
        public double TotalPl { get; set; }
        public double TotalPlPercentage { get; set; }
        public double TotalUnits { get; set; }
        public double Value { get; set; }
    }
}