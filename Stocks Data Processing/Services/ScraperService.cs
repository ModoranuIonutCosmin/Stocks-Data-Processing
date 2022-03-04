using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Exceptions;
using Stocks_Data_Processing.Interfaces.Services;

namespace Stocks_Data_Processing.Services
{
    public class ScraperService : IScraperService
    {
        private readonly HttpClient httpClient;


        public ScraperService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private async Task<string> GetPageHTMLSource(string link)
        {
            //Incearca sa faca GET la sursa paginii pe care avem informatiile necesare...
            var response = await httpClient.GetAsync(link);

            response.EnsureSuccessStatusCode();

            string pageSource = await response.Content.ReadAsStringAsync();

            return pageSource;
        }

        public async Task<decimal> GetNumericFieldValueByHtmlClassesCombination(string link, List<string> classes)
        {
            var pageSource = await GetPageHTMLSource(link);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            //Si apoi cauta elementul ce contine pretul unui share
            //in functie de unele trasaturi care identifica unic elementul.
            var htmlElements = htmlDoc.DocumentNode.Descendants(0)
                .Where(n => n.GetClasses()
                             .SequenceEqual(classes));

            if (!htmlElements.Any())
                throw new ScrapeNoElementException($"{nameof(GetNumericFieldValueByHtmlClassesCombination)} : No element" +
                    $" to scrape found");

            var valueString = htmlElements.First().InnerText;

            if (!valueString.ParseCurrency(out decimal valueNumeric))
                throw new CurrencyParseException($"{nameof(GetNumericFieldValueByHtmlClassesCombination)} : Couldn't parse currency!");

            return valueNumeric;
        }
    }
}
