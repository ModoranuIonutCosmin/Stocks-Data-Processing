using System.Globalization;

namespace Stocks.General.ExtensionMethods;

public static class CurrencyParserHelpers
{
    public static bool ParseCurrency(this string currency,  out decimal parsedValue)
    {
        //Parseaza valoarea ca double si salveaza statusul in aceasta variabila
        //iar valoarea, in caz de success in currentPrice.
        try
        {
            var successfulParse =
                decimal.Parse(currency, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, new CultureInfo("en-US"));
            parsedValue = successfulParse;
        }
        catch
        {
            parsedValue = 0;
            
            return false;
        }
        return true;
    }
}