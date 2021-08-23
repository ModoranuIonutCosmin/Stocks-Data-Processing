using Stocks_Data_Processing.Models;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
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
        #region Private members
        private readonly ICurrentStockInfoDataScraperService _currentStockInfoDataScraper;
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
            StocksMarketContext stocksContext)
        {
            _currentStockInfoDataScraper = currentStockInfoDataScraper;
            _stocksContext = stocksContext;
        }

        #endregion


        #region Main functionality

        /// <summary>
        /// Updateaza valorile stock-urilor urmarite in baza de date.
        /// </summary>
        public async Task UpdateStockDataAsync()
        {
            //Obtine date despre stock-urile companiilor urmarite.
            var stockData = await _currentStockInfoDataScraper.GatherAllAsync();

            ///Obtine lista acestor companii din enumul <see cref="StocksTicker"/>
            var companies = Enum.GetValues(typeof(StocksTicker)).Cast<StocksTicker>()
                                    .Select(s => s.ToString()).ToList();

            if (!_stocksContext.Companies.Any())
            //Daca tabelul companiilor (din BD) nu contine nicio companie...
            {
                //... introduce toate companiile urmarite.
                _stocksContext.Companies.AddRange(companies.Select(ticker => new Company()
                {
                    Name = $"{ticker} company",
                    Ticker = ticker
                }));
            }

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

            //Incepe tranzactia formata local si in BD si-i face SAVE.
            _stocksContext.SaveChanges();
        }
    }  
    #endregion
}
