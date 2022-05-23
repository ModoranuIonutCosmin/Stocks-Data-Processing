using System;
using System.Globalization;

namespace Stocks.General.ExtensionMethods;

public static class DateFormatterHelpers
{
    public static string ToUSDashDateFormat(this DateTimeOffset date)
        => date.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}