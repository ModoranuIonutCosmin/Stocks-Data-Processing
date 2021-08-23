namespace Stocks_Data_Processing
{
    public class Application : IApplication
    {
        private readonly IStocksDataHandlingLogic stocksDataHandlingLogic;

        public Application(IStocksDataHandlingLogic stocksDataHandlingLogic)
        {
            this.stocksDataHandlingLogic = stocksDataHandlingLogic;
        }
        public void Run()
        {
           stocksDataHandlingLogic.StartAllFunctions().Wait();
        }
    }
}
