using Stocks.General.ExtensionMethods;
using System;

class Program
{
    static void Main()
    {
        DateTimeOffset a = DateTimeOffset.UtcNow.AddDays(2);

        Console.WriteLine(a.GetNextStockMarketTime(TimeSpan.FromMinutes(1)));
    }
}

