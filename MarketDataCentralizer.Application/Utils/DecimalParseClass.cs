using System.Runtime.CompilerServices;

namespace MarketDataCentralizer.Application.Utils
{
    public static class DecimalParseClass
    {

        public static decimal ParseDecimal(this string? value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");

            if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"Unable to parse '{value}' as a decimal.");
            }
        }

    }
}
