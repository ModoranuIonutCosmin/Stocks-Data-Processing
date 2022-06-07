using System;

namespace Stocks.General.ExtensionMethods;

public static class TimespanParser
{
    public static long ParseTimeSpanTicks(string interval)
    {
        var length = interval.Length - 1;
        var value = interval.Substring(0, length);
        var type = interval.Substring(length, 1);

        switch (type)
        {
            case "d": return TimeSpan.FromDays(double.Parse(value)).Ticks;
            case "h": return TimeSpan.FromHours(double.Parse(value)).Ticks;
            case "m": return TimeSpan.FromMinutes(double.Parse(value)).Ticks;
            case "s": return TimeSpan.FromSeconds(double.Parse(value)).Ticks;
            case "f": return TimeSpan.FromMilliseconds(double.Parse(value)).Ticks;
            case "z": return TimeSpan.FromTicks(long.Parse(value)).Ticks;
            default: return TimeSpan.FromDays(double.Parse(value)).Ticks;
        }
    }
}