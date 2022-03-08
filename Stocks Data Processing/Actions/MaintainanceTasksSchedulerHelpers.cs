using System.Collections.Generic;

namespace Stocks_Data_Processing.Actions
{
    internal static class MaintainanceTasksSchedulerHelpers
    {

        public const string TaxesCollectJob = "taxesCollect";
        public const string PredictionsRefreshJob = "predictionsRefresh";
        public const string CurrentSummariesJob = "currentSummaries";
        public const string CurrentStocksJob = "currentStocks";
        public const string TransactionMonitorJob = "transactionsMonitor";

        public static List<string> allTasksNames = new() { TaxesCollectJob, PredictionsRefreshJob,
            CurrentSummariesJob, CurrentStocksJob, TransactionMonitorJob };
    }
}