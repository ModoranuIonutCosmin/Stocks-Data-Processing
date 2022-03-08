using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stocks.General;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Interfaces.Services;
using Stocks_Data_Processing.Models;

namespace Stocks_Data_Processing.Services
{
    /// <summary>
    /// Serviciu ce face rost de valorile curente ale companiilor 
    /// ale caror tickere se afla in enum-ul <see cref="StocksTicker"/>
    /// din diverse surse precum Google Finance sau  Y! Finance 
    /// in functie de disponibilitatea lor pe oricare din aceste platforme
    /// </summary>
    public class CurrentStockInfoDataScraperService : ICurrentStockInfoDataScraperService
    {

        #region Private members - services

        private readonly ICurrentStockInfoYahooScraperService _currentStockYahooService;
        private readonly ICurrentStockInfoGoogleScraperService _currentStockGoogleService;
        private readonly ILogger<CurrentStockInfoDataScraperService> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentStockYahooService">Serviciul ce obtine date despre pretul 
        /// curent de pe Y! Finance</param>
        /// 
        /// <param name="currentStockGoogleService">Serviciul ce obtine date despre pretul 
        /// curent de pe Google Finance</param>
        /// <param name="logger">Logger-ul principal spre consola</param>
        public CurrentStockInfoDataScraperService(
           ICurrentStockInfoYahooScraperService currentStockYahooService,
           ICurrentStockInfoGoogleScraperService currentStockGoogleService,
           ILogger<CurrentStockInfoDataScraperService> logger)
        {
            _currentStockYahooService = currentStockYahooService;
            _currentStockGoogleService = currentStockGoogleService;
            _logger = logger;
        }
        #endregion

        #region Main functionality

        /// <summary>
        /// Obtine datele numerice referitoate la valoarea stock-urile unei singure companii.
        /// </summary>
        /// <param name="ticker">Simbolul companiei pentru care luam valorile.</param>
        /// <returns>Grupare ce contine informatiile utile despre rezultat-ul cautat.</returns>
        public async Task<StockCurrentInfoResponse> GatherAsync(string ticker)
        {
            //Incearca sa faca scrape la Google Finance si apoi salveaza datele obtinute in urma
            //procesului inclusiv daca a fost cu success.
            var stockDataResponse = await _currentStockGoogleService.GatherAsync(ticker);

            if (!stockDataResponse.Successful)
            //Daca scraping-ul la Google Finance a esuat...
            {
                //... obtine datele de pe Yahoo Finance.
                stockDataResponse = await _currentStockYahooService.GatherAsync(ticker);
            }

            if (!stockDataResponse.Successful)
            //Daca amandoua modalitati esueaza...
            {
                //... logheaza faptul ca a esuat.
                _logger.LogCritical($"Gathering data failed for ticker { ticker }");
            }

            return stockDataResponse;
        }

        /// <summary>
        /// Obtine in paralel datele in legatura cu valorile curente
        /// ale stock-urilor pentru toate companiile din <see cref="WatchList"/>
        /// </summary>
        /// <returns>Returneaza o lista cu datele pentru fiecare simbol</returns>
        /// <remarks>Lista obtinuta e defapt System.Array</remarks>
        /// <seealso cref="StocksTicker"/>
        public async Task<IList<StockCurrentInfoResponse>> GatherAllAsync()
        {
            var GatherTasks = new List<Task<StockCurrentInfoResponse>>();
            var WatchList = TickersHelpers.GatherAllTickers();

            foreach (var ticker in WatchList)
            //Pentru fiecare companie pe care o urmarim...
            {
                //... porneste procesul de obtinerea valorii stock-ului.
                GatherTasks.Add(GatherAsync(ticker));
            }

            //Returneaza valorile pentru fiecare companie cand se termina task-urile
            //aferente obtinerii lor.
            return await Task.WhenAll(GatherTasks);
        }
        #endregion

    }
}
