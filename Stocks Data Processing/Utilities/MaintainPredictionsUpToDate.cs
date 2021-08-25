using Microsoft.Extensions.Logging;
using Stocks.General;
using StocksProccesing.Relational.DataAccess;
using StocksProccesing.Relational.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    public class MaintainPredictionsUpToDate : IMaintainPredictionsUpToDate
    {
        private readonly StocksMarketContext _stockContext;
        private readonly IPredictionsService _predictionsService;
        private readonly ILogger<MaintainPredictionsUpToDate> _logger;

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

        public MaintainPredictionsUpToDate(StockContextFactory stockContextFactory,
            IPredictionsService predictionsService,
            ILogger<MaintainPredictionsUpToDate> logger)
        {
            _stockContext = stockContextFactory.Create();
            _predictionsService = predictionsService;
            _logger = logger;
        }

        public async Task UpdatePredictionsAsync()
        {
            foreach (var ticker in WatchList)
            {
                var predictionsChunk = (await _predictionsService.Predict(ticker))
                    .Select(k => new StocksPriceData()
                    {
                        Prediction = true,
                        Price = Math.Round(k.Price, 2),
                        Date = k.Date,
                        CompanyTicker = ticker
                    });

                var max = predictionsChunk.Max();
                var min = predictionsChunk.Min();


                if (max.Price > 10000 || min.Price <= 0)
                {
                    _logger.LogError($"Bad values, Was {max.Price} for max and {min.Price} for min!");
                    continue;
                } 
                else
                {
                    _logger.LogWarning($"Values are {max.Price} for max and {min.Price} for min!");
                }

                await _stockContext.PricesData.AddRangeAsync(predictionsChunk);
                await _stockContext.SaveChangesAsync();

                _logger.LogWarning($"Updated predictions for {ticker}!");
            }
        }
    }
}
