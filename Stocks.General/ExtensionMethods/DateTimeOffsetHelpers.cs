using System;

namespace Stocks.General.ExtensionMethods;

public static class DateTimeOffsetHelpers
{
    public static DateTimeOffset RoundUp(this DateTimeOffset dto, TimeSpan d)
    {
        var dt = dto.UtcDateTime;

        var modTicks = dt.Ticks % d.Ticks;
        var delta = modTicks != 0 ? d.Ticks - modTicks : 0;

        return new DateTime(dt.Ticks + delta, dt.Kind);
    }

    public static DateTimeOffset RoundDown(this DateTimeOffset dto, TimeSpan d)
    {
        var dt = dto.UtcDateTime;

        var delta = dt.Ticks % d.Ticks;
        return new DateTime(dt.Ticks - delta, dt.Kind);
    }

    public static bool IsWorkDay(this DateTimeOffset dto)
    {
        return dto.DayOfWeek != DayOfWeek.Sunday && dto.DayOfWeek != DayOfWeek.Saturday;
    }

    public static bool IsDateDuringStockMarketOpenTimeframe(this DateTimeOffset dto)
    {
        return dto.DayOfWeek < DayOfWeek.Saturday &&
               dto.DayOfWeek > DayOfWeek.Sunday &&
               dto.TimeOfDay >= new TimeSpan(8, 0, 0) &&
               dto.TimeOfDay <= new TimeSpan(23, 59, 59);
    }

    /// <summary>
    ///     Sets the time of the current date with minute precision.
    /// </summary>
    /// <param name="current">The current date.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <returns>A DateTimeOffset.</returns>
    public static DateTimeOffset SetTime(this DateTimeOffset current, int hour, int minute)
    {
        return SetTime(current, hour, minute, 0, 0);
    }

    /// <summary>
    ///     Sets the time of the current date with millisecond precision.
    ///     Sees dateTimeOffset as UTC regardless.
    /// </summary>
    /// <param name="current">The current date.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <param name="second">The second.</param>
    /// <param name="millisecond">The millisecond.</param>
    /// <returns>A DateTimeOffset.</returns>
    public static DateTimeOffset SetTime(this DateTimeOffset current, int hour, int minute, int second, int millisecond)
    {
        return new DateTime(current.Year, current.Month, current.Day,
            hour, minute, second, millisecond, DateTimeKind.Utc);
    }

    public static DateTimeOffset GetNextStockMarketTime(this DateTimeOffset dto, TimeSpan minimumDelay)
    {
        var dt = dto.Add(minimumDelay);

        var isWorkDay = dt.IsWorkDay();
        var betweenTradingHours = dt.IsDateDuringStockMarketOpenTimeframe();
        var stockMarketOpen = isWorkDay && betweenTradingHours;


        if (!stockMarketOpen)
            //Daca nu e in intervalul de trade...
        {
            if (!isWorkDay)
                //...din cauza ca e in weekend...
            {
                //Gaseste data in viitor in ziua luni.
                var daysToAdd = ((int) DayOfWeek.Monday - (int) dt.DayOfWeek + 7) % 7;
                dt = dt.AddDays(daysToAdd);
                dt = dt.SetTime(8, 0);

                return dt;
            }

            //...din cauza ca nu e intre inceput premarket si sfarsit aftermarket
            dt = dt.SetTime(8, 0);
        }

        return dt;
    }

    public static DateTimeOffset GetClosestPreviousStockMarketDateTime(this DateTimeOffset dto)
    {
        var date = dto;

        switch (date.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                date = date.AddDays(-1);
                break;
            case DayOfWeek.Sunday:
                date = date.AddDays(-2);
                break;
        }

        return date.SetTime(8, 0);
    }

    public static decimal GetBusinessDays(DateTimeOffset startD, DateTimeOffset endD)
    {
        var calcBusinessDays =
            1 + ((endD - startD).TotalDays * 5 -
                 (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

        if (endD.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
        if (startD.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

        return (decimal) calcBusinessDays;
    }

    public static string ExtractDate(this DateTimeOffset dto)
    {
        return dto.Date.ToShortDateString();
    }
}