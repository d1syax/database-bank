namespace MyBank.Domain.Constants;

public static class CurrencyConstants
{
    public const string UAH = "UAH";
    public const string USD = "USD";
    public const string EUR = "EUR";
    
    private static readonly HashSet<string> SupportedSet = new() { UAH, USD, EUR };
    
    public static bool IsValid(string currency)
    {
        return !string.IsNullOrWhiteSpace(currency) && SupportedSet.Contains(currency.ToUpper());
    }
}