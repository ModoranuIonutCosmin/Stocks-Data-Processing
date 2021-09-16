using System;

namespace Stocks.General.ExtensionMethods
{
    public static class DecimalNumbersHelpers
    {

        public static double TruncateToDecimalPlaces(this double number, int decimaPlaces)
        {
            return Convert.ToDouble(number.ToString("N" + decimaPlaces));
        }
    }
}
