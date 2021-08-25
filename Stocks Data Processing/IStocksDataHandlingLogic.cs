using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Data_Processing
{
    public interface IStocksDataHandlingLogic
    {

        Task StartMantainingCurrentStocksData();
        Task StartPredictionEngine();

        public async Task StartAllFunctions()
        {
            await Task.WhenAll(new List<Task>{ StartMantainingCurrentStocksData(), StartPredictionEngine() });
        }
    }
}