using System;
using System.Collections.Generic;
using System.Linq;

namespace Stocks.General.ExtensionMethods;

public static class TickersHelpers
{
    public static List<string> GatherAllTickers()
    {
        return Enum.GetValues(typeof(StocksTicker)).Cast<StocksTicker>()
            .Select(s => s.ToString()).ToList();
    }
}