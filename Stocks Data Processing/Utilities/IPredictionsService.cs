using StocksProcessing.ML;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public interface IPredictionsService
    {
        int Horizon { get; set; }

        Task<List<PredictionResult>> Predict(string ticker);
    }
}