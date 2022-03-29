using System;
using System.Threading.Tasks;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Interfaces.Services;
using Stocks_Data_Processing.Models;

namespace Stocks_Data_Processing.Services
{
    /// <summary>
    /// Serviciu ce se ocupa cu obtinerea datelor referitoare la valoarea stock-ului
    /// facand scrape la Yahoo Finance.
    /// </summary>
    public class CurrentStockInfoYahooScraperService : ICurrentStockInfoYahooScraperService
    {
        private readonly IScraperService scraper;
        private const string YAHOO_FINANCE_LINK = "https://finance.yahoo.com/quote/";

        public CurrentStockInfoYahooScraperService(
            IScraperService scraper)
        {
            this.scraper = scraper;
        }

        #region Main functionality - Actual WebScraping
        /// <summary>
        /// Incearca obtinerea datelor la momentul curent referitoate la pretul unui share
        /// a unei companii de pe stock market.
        /// </summary>
        /// <param name="ticker">Simbolul companiei pentru care aflam aceste date.</param>
        /// <returns>Obiect indicand success-ul si rezultatul metodei</returns>
        public async Task<StockCurrentInfoResponse> GatherAsync(string ticker)
        {
            var stocksInfoResponse = new StockCurrentInfoResponse
            {
                //Asociaza situatiei simbolul companiei pentru care este observata.
                Ticker = ticker
            };

            try
            {
                stocksInfoResponse.Current = 
                    await scraper.GetNumericFieldValueByHtmlClassesCombination(BuildResourceLink(ticker),
                    new() { "Trsdu(0.3s)", "Fw(b)", "Fz(36px)", "Mb(-4px)", "D(ib)" });
                stocksInfoResponse.DateTime = DateTimeOffset.UtcNow.RoundDown(TimeSpan.FromMinutes(1));
            }

            catch (Exception ex)
            {
                //Daca esueaza GET-ul, asociaza exceptia si returneaza statusul.
                stocksInfoResponse.Exception = ex;
                return stocksInfoResponse;
            }

            return stocksInfoResponse;
        }
        #endregion

        #region Main functionality - Building request link for stock resource.
        /// <summary>
        /// Construieste link-ul in functie de simbolul companiei
        /// </summary>
        /// <param name="ticker">Simbolul companiei</param>
        /// <returns></returns>
        public string BuildResourceLink(string ticker)
        {
            return $"{YAHOO_FINANCE_LINK}{ticker}";
        }
        #endregion
    }
}
