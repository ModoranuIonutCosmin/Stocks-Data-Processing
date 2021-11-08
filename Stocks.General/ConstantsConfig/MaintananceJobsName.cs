using System.Collections.Generic;

namespace Stocks.General.ConstantsConfig
{
    public static class MaintananceJobsName
    {
        public const string CurrentStocksValuesJob  = "CurrentStock";
        public const string PredictionsJob          = "Predictions";
        public const string TaxesCollectingJob      = "Taxes";
        public const string TransactionClosingJob   = "Transactions";

        public static Dictionary<string, string> AllJobs = new Dictionary<string, string>()
        {
            {"CurrentStocksValuesJob", CurrentStocksValuesJob },
            {"PredictionsJob", PredictionsJob },
            {"TaxesCollectingJob", TaxesCollectingJob },
            {"TransactionClosingJob", TransactionClosingJob }
        };
    }
}
