using System;

namespace Stocks.General.ConstantsConfig
{
    public static class MaintenanceRoutinesPeriod
    {
        public static TimeSpan PredictionsJob = TimeSpan.FromDays(7);
        public static TimeSpan TaxesCollectingJob = TimeSpan.FromDays(7);
        public static TimeSpan StocksPriceCollectingJob = TimeSpan.FromMinutes(1);
        public static TimeSpan TransactionsMonitorJob = TimeSpan.FromMinutes(1);
    }
}
