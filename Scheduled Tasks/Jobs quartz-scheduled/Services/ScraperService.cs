using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Stocks.General.ExtensionMethods;
using Stocks_Data_Processing.Exceptions;
using Stocks_Data_Processing.Interfaces.Services;

namespace Stocks_Data_Processing.Services;

public class ScraperService : IScraperService
{
    public async Task<List<decimal>> ExtractNumericFields(string url, string xPath)
    {
        var htmlDoc = await new HtmlWeb().LoadFromWebAsync(url);
        
        var htmlNodes = htmlDoc.DocumentNode
            .SelectNodes(xPath)
            .ToList();

        if (!htmlNodes.Any())
            throw new ScrapeNoElementException($"{nameof(ExtractNumericFields)} : No element" +
                                               " to scrape found");

        List<decimal> result = new List<decimal>();
        
        htmlNodes.ForEach(node =>
        {
            bool success = node.InnerText.ParseCurrency(out var valueNumeric);
            
            if (success)
            {
                result.Add(valueNumeric);
            }
        });
        
        return result;
    }

}