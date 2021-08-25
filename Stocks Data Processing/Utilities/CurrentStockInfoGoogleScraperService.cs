using HtmlAgilityPack;
using Stocks.General;
using Stocks_Data_Processing.ExtensionMethods;
using Stocks_Data_Processing.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stocks_Data_Processing.Utilities
{
    /// <summary>
    /// Serviciu ce se ocupa cu obtinerea datelor referitoare la valoarea stock-ului
    /// facand scrape la Google Finance.
    /// </summary>
    public class CurrentStockInfoGoogleScraperService : ICurrentStockInfoGoogleScraperService
    {
        #region Private members
        private readonly HttpClient httpClient; 
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient">Client ce face diverse request-uri HTTP</param>
        public CurrentStockInfoGoogleScraperService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        #endregion

        #region Main functionality - Actual WebScraping


        /// <summary>
        /// Incearca obtinerea datelor la momentul curent referitoate la pretul unui share
        /// a unei companii de pe stock market.
        /// </summary>
        /// <param name="ticker">Simbolul companiei pentru care aflam aceste date.</param>
        /// <returns>Obiect indicand success-ul si rezultatul metodei</returns>
        public async Task<StockCurrentInfoResponse> GatherAsync(string ticker)
        {
            var pageSource = default(string);

            var stocksInfoResponse = new StockCurrentInfoResponse();

            if (!Enum.TryParse(ticker, out StocksTicker tickerEnum))
            //Daca simbolul companiei nu exista in lista tickerelor ce ne intereseaza...
            {
                // ...asociaza exceptie noua si returneaza statusul.
                stocksInfoResponse.Exception = new Exception("Invalid ticker option");

                return stocksInfoResponse;
            }

            //Asociaza situatiei simbolul companiei pentru care este observata.
            stocksInfoResponse.Ticker = tickerEnum;

            try
            {
                //Incearca sa faca GET la sursa paginii pe care avem informatiile necesare...
                var response = await httpClient.GetAsync(BuildResourceLink(ticker));

                response.EnsureSuccessStatusCode();

                pageSource = await response.Content.ReadAsStringAsync();
            }

            catch (Exception ex)
            {
                //Daca esueaza GET-ul, asociaza exceptia si returneaza statusul.
                stocksInfoResponse.Exception = ex;
                return stocksInfoResponse;
            }

            //Asociaza data la care s-au obtinut valorile in format UTC.
            stocksInfoResponse.DateTime = DateTimeOffset.UtcNow.RoundDown(TimeSpan.FromMinutes(1));

            //Incarca documentul html in memorie.
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            //Si apoi cauta elementul ce contine pretul unui share
            //in functie de unele trasaturi care identifica unic elementul.
            var htmlElements = htmlDoc.DocumentNode.
                    Descendants(0)
                .Where(n => n.HasClass("YMlKec") &&
                            n.HasClass("fxKbKc"));

            if (!htmlElements.Any())
            //Daca nu gasim vreun element...
            {
                // ...asociaza exceptie noua si returneaza statusul.
                stocksInfoResponse.Exception = new Exception("Couldn't find the html element");
                return stocksInfoResponse;
            }

            ///Salveaza continutul elementului HTML ce contine valoarea
            ///<remarks>Va avea formatul $100.40</remarks>
            string value = htmlElements.First().InnerText;

            value = value.Trim('$');
            value = value.Replace('.', ',');

            //Parseaza valoarea ca double si salveaza statusul in aceasta variabila
            //iar valoarea, in caz de success in currentPrice.
            var successfulParse = double.TryParse(value, out double currentPrice);

            if (!successfulParse)
            //Daca nu s-a parsat cu success...
            {
                // ...asociaza exceptie noua si returneaza statusul.
                stocksInfoResponse.Exception = new Exception("Double parsing failed");
            }
            else
            //Altfel...
            {
                //... salveaza valoarea astfel obtinuta in obiectul rezultant.
                stocksInfoResponse.Current = currentPrice;
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
            return $"https://www.google.com/finance?q={ticker}";
        } 
        #endregion
    }
}
