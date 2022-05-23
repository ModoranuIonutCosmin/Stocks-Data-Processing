using System.Collections.Generic;

namespace StockBulkGatherer;

public class StockPricesAPIModel
{
    public string Ticker { get; set; }
    public List<PriceDataAPIModel> Results { get; set; }
}