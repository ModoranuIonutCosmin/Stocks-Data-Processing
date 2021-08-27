using Microsoft.Extensions.Logging;
using Quartz;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    /// <summary>
    /// Serviciu ce updateaza valorile stock-urilor urmarite in baza de date.
    /// </summary>
    /// <returns></returns>
    public class MaintainCurrentStockData : IMaintainCurrentStockData, IJob
    {
        #region Private members
        private readonly ICurrentStockInfoDataScraperService _currentStockInfoDataScraper;
        private readonly StockContextFactory _stocksContextFactory;
        private readonly ILogger<MaintainCurrentStockData> _logger;
        private readonly StocksMarketContext _stocksContext;
        #endregion

        #region Constructor

        /// <summary>
        /// Injecteaza serviciile necesare.
        /// </summary>
        /// <param name="currentStockInfoDataScraper">Serviciu care face scrape 
        /// si returneaza rezultate legate de pretul stock-urilor</param>
        /// <param name="stocksContext">Context-ul bazei de date aferent aplicatiei.</param>
        public MaintainCurrentStockData(
            ICurrentStockInfoDataScraperService currentStockInfoDataScraper,
            StockContextFactory stocksContextFactory,
            ILogger<MaintainCurrentStockData> logger)
        {
            _currentStockInfoDataScraper = currentStockInfoDataScraper;
            _stocksContextFactory = stocksContextFactory;
            _logger = logger;
            _stocksContext = _stocksContextFactory.Create();
        }



        #endregion

        #region Main functionality


        public async Task Execute(IJobExecutionContext context)
        {
            await UpdateStockDataAsync();
        }

        /// <summary>
        /// Updateaza valorile stock-urilor urmarite in baza de date.
        /// </summary>
        public async Task UpdateStockDataAsync()
        {
            _logger.LogWarning("Starting to update current stock data");

            //Obtine date despre stock-urile companiilor urmarite.
            var stockData = await _currentStockInfoDataScraper.GatherAllAsync();

            //Formeaza lista de randuri ce trebuie adaugate in tabelul cu preturi.
            var stocksTableEntries = stockData.Select(response =>
                new StocksPriceData
                {
                    Price = response.Current,
                    Prediction = false,
                    Date = response.DateTime,
                    CompanyTicker = response.Ticker.ToString()
                }).ToList();

            //Adauga aceste randuri in tabel.
            await _stocksContext.AddRangeAsync(stocksTableEntries);

            //Sterge toate predictiile mai vechi decat cea mai noua valoare
            //realizata in acest moment.
            var oldPredictions = _stocksContext.PricesData
                .Where(p => (p.Prediction && p.Date < DateTimeOffset.UtcNow));

            _stocksContext.PricesData.RemoveRange(oldPredictions);

            _stocksContext.SaveChanges();
        }
    }
    #endregion
}
