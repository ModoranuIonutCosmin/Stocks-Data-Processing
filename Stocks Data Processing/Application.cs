using System.Threading.Tasks;

namespace Stocks_Data_Processing
{
    public class Application : IApplication
    {
        private readonly IStocksDataHandlingLogic stocksDataHandlingLogic;

        public Application(IStocksDataHandlingLogic stocksDataHandlingLogic)
        {
            this.stocksDataHandlingLogic = stocksDataHandlingLogic;
        }
        public async Task Run()
        {
            await stocksDataHandlingLogic.StartAllFunctions();
        }
    }
}
