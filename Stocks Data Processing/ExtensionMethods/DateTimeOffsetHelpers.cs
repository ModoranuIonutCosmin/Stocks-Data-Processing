using System;

namespace Stocks_Data_Processing.ExtensionMethods
{
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
    }
}
