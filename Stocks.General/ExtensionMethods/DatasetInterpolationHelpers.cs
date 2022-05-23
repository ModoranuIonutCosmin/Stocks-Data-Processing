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

        for (var i = 0; i < priceData.Count - 1; i++)
        {
            current = priceData[i];
            next = priceData[i + 1];
            currentDt = current.Date;
            nextDt = next.Date;

            if (currentDt.Day == nextDt.Day)
            {
                var deltaTime = nextDt - currentDt;
                decimal source;

                source = current.Price;

                for (var j = 1; j < deltaTime.TotalMinutes; ++j)
                {
                    ++i;

                    var fillingRow = new StocksPriceData
                    {
                        CompanyTicker = current.CompanyTicker,
                        Price = source,
                        Date = currentDt.AddMinutes(j),
                        Prediction = false
                    };

                    priceData.Insert(i, fillingRow);
                }
            }
        }

        return priceData;
    }
}