using System.Threading.Tasks;
using Stocks.General.Models;

namespace StocksFinalSolution.BusinessLogic.Interfaces.Services
{
    public interface IPredictionsDataService
    {
        Task<StocksPredictionsPaginatedDTO> GatherPredictions(string predictionEngine,
            string ticker, int page, int count);
    }
}