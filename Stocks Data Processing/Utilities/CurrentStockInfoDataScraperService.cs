using Microsoft.Extensions.Logging;
using Stocks.General;
using Stocks_Data_Processing.Models;
using StocksProccesing.Relational.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
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


        #region Private members - Variables

        /// <summary>
        /// Tine intr-o variabila statica lista companiilor pe care le
        /// urmarim spre a le updata datele in BD.
        /// See creaza pornind de la toate field-urile enumeratiei <see cref="StocksTicker"/>
        /// </summary>
        private static readonly List<string> WatchList
            = Enum.GetValues(typeof(StocksTicker)).Cast<StocksTicker>()
                                                .Select(s => s.ToString()).ToList(); 
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
