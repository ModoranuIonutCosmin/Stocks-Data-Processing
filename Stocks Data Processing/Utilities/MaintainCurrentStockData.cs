using Microsoft.Extensions.Logging;
using Quartz;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.DataAccess.V1.Repositories;
using StocksProccesing.Relational.Extension_Methods;
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
    public class MaintainCurrentStockData : IMaintainCurrentStockData
    {
        private readonly ICompaniesRepository companiesRepository;
        #region Private members
        private readonly IStockPricesRepository stockPricesRepository;
        private readonly ICurrentStockInfoDataScraperService _currentStockInfoDataScraper;
        private readonly ILogger<MaintainCurrentStockData> _logger;
        #endregion

        #region Constructor

        /// <summary>
        /// Injecteaza serviciile necesare.
        /// </summary>
        /// <param name="currentStockInfoDataScraper">Serviciu care face scrape 
        /// si returneaza rezultate legate de pretul stock-urilor</param>
        /// <param name="stocksContext">Context-ul bazei de date aferent aplicatiei.</param>
        public MaintainCurrentStockData(
            ICompaniesRepository companiesRepository,
            IStockPricesRepository stockPricesRepository,
            ICurrentStockInfoDataScraperService currentStockInfoDataScraper,
            ILogger<MaintainCurrentStockData> logger)
        {
            this.companiesRepository = companiesRepository;
            this.stockPricesRepository = stockPricesRepository;
            _currentStockInfoDataScraper = currentStockInfoDataScraper;
            _logger = logger;
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
            _logger.LogWarning($"[Current prices maintain task] Starting to update current stock data {DateTimeOffset.UtcNow}");

            companiesRepository.EnsureCompaniesDataExists();

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
            await stockPricesRepository.AddRangeAsync(stocksTableEntries);

            await stockPricesRepository.DeleteWhereAsync(p => p.Prediction && p.Date < DateTimeOffset.UtcNow);

            _logger.LogWarning($"[Current prices maintain task] Done to update current stock data {DateTimeOffset.UtcNow}");

        }
    }
    #endregion
}
