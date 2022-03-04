using System;

namespace Stocks.General.ExtensionMethods
{
    public static class DecimalNumbersHelpers
    {
        public static decimal TruncateToDecimalPlaces(this decimal number, int decimalPlaces)
        {
            return Convert.ToDecimal(number.ToString("N" + decimalPlaces));
        }
    }
}
