using System;
using System.Collections.Generic;
using StocksProccesing.Relational.Model;

namespace Stocks.General.ExtensionMethods;

public static class DatasetInterpolationHelpers
{
    public static List<StocksPriceData> InterpolateMissingValues(this List<StocksPriceData> priceData)
    {
        StocksPriceData current;
        StocksPriceData next;
        DateTimeOffset currentDt;
        DateTimeOffset nextDt;

        for (int entryIndex = 0; entryIndex < priceData.Count - 1; entryIndex++)
        {
            current = priceData[entryIndex];
            next = priceData[entryIndex + 1];
            currentDt = current.Date;
            nextDt = next.Date;

            if (currentDt.Day == nextDt.Day)
            {
                var deltaTime = nextDt - currentDt;

                decimal source = current.Price;

                for (var occurenceIndex = 1; occurenceIndex < deltaTime.TotalMinutes; ++occurenceIndex)
                {
                    ++entryIndex;

                    var fillingRow = new StocksPriceData
                    {
                        CompanyTicker = current.CompanyTicker,
                        Price = source,
                        Date = currentDt.AddMinutes(occurenceIndex),
                        Prediction = false
                    };

                    priceData.Insert(entryIndex, fillingRow);
                }
            }
        }

        return priceData;
    }
}