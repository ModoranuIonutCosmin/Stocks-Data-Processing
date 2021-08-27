using StocksProcessing.ML;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public class PredictionsService : IPredictionsService
    {
        private PredictionEngine PredictionEngine { get; set; }

        public int Horizon { get; set; } = 5 * 16 * 60;
        public PredictionsService()
        {
            PredictionEngine = new PredictionEngine();
        }
        public async Task<List<PredictionResult>> Predict(string ticker)
        {
            return await Task.Run(() => PredictionEngine.Predict(ticker, Horizon));
        }
    }
}
