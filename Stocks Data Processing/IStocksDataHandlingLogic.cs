using System.Threading.Tasks;

namespace Stocks_Data_Processing
{
    public interface IStocksDataHandlingLogic
    {
        Task StartMantainingCurrentStocksData();
        Task StartPredictionEngine();

        Task StartAllFunctions();
    }
}