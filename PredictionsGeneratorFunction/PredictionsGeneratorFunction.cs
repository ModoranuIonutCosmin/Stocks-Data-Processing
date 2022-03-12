﻿using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Stocks_Data_Processing.Interfaces.Jobs;
using System.Threading.Tasks;

namespace PredictionsGeneratorFunction
{
    public class PredictionsGeneratorFunction
    {
        private readonly IMaintainPredictionsUpToDate maintainPredictionsUpToDate;

        public PredictionsGeneratorFunction(IMaintainPredictionsUpToDate maintainPredictionsUpToDate)
        {
            this.maintainPredictionsUpToDate = maintainPredictionsUpToDate;
        }

        [FunctionName("PredictionsGeneratorFunction")]
        public async Task Run([TimerTrigger("0 8 * * * SAT")] TimerInfo myTimer, ILogger log)
        {
            await maintainPredictionsUpToDate.Execute(default);
        }
    }
}
