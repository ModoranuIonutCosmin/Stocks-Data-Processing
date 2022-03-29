using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Interfaces.Services;
using Stocks_Data_Processing.Models;

namespace Stocks_Data_Processing.Services;

/// <summary>
///     Serviciu ce se ocupa cu obtinerea datelor referitoare la valoarea stock-ului
///     facand scrape la Google Finance.
/// </summary>
public class CurrentStockInfoGoogleScraperService : ICurrentStockInfoGoogleScraperService
{
    #region Constructor

    /// <summary>
    /// </summary>
    /// <param name="httpClient">Client ce face diverse request-uri HTTP</param>
    public CurrentStockInfoGoogleScraperService(
        IScraperService scraper)
    {
        this.scraper = scraper;
    }

    #endregion

    #region Main functionality - Actual WebScraping

    /// <summary>
    ///     Incearca obtinerea datelor la momentul curent referitoate la pretul unui share
    ///     a unei companii de pe stock market.
    /// </summary>
    /// <param name="ticker">Simbolul companiei pentru care aflam aceste date.</param>
    /// <returns>Obiect indicand success-ul si rezultatul metodei</returns>
    public async Task<StockCurrentInfoResponse> GatherAsync(string ticker)
    {
        var stocksInfoResponse = new StockCurrentInfoResponse
        {
            Ticker = ticker
        };

        try
        {
            stocksInfoResponse.Current =
                await scraper.GetNumericFieldValueByHtmlClassesCombination(BuildResourceLink(ticker),
                    new List<string> {"YMlKec", "fxKbKc"});
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
    ///     Construieste link-ul in functie de simbolul companiei
    /// </summary>
    /// <param name="ticker">Simbolul companiei</param>
    /// <returns></returns>
    public string BuildResourceLink(string ticker)
    {
        return $"{GOOGLE_LINK}{ticker}";
    }

    #endregion

    #region Private members

    private readonly IScraperService scraper;
    private const string GOOGLE_LINK = "https://www.google.com/finance?q=";

    #endregion
}