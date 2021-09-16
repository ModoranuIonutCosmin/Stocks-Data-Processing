using System;

namespace Stocks_Data_Processing.Models
{
    public static class MaintenanceRoutinesPeriod
    {
        public static TimeSpan PredictionsJob = TimeSpan.FromDays(7);
        public static TimeSpan TaxesCollectingJob = TimeSpan.FromDays(7);
    }
}
