using System.Globalization;

namespace Stocks.General.ExtensionMethods
{
    public static class CurrencyParserHelpers
    {
        public static bool ParseCurrency(this string currency, out decimal parsedValue)
        {
            var value = currency.Trim('$');

            //Parseaza valoarea ca double si salveaza statusul in aceasta variabila
            //iar valoarea, in caz de success in currentPrice.
            var successfulParse = decimal.TryParse(value, NumberStyles.Currency,
                new CultureInfo("en-US"), out decimal currentPrice);

            parsedValue = currentPrice;

            return successfulParse;
        }
    }
}
