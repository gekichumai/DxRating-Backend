namespace DxRating.Services.Authentication.Utils;

public static class EthereumUtils
{
    public static string NormalizeAddress(string address)
    {
        // Make sure it starts with 0x
        var normalizedAddress = address;
        if (!normalizedAddress.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            normalizedAddress = "0x" + normalizedAddress;
        }

        normalizedAddress = normalizedAddress.ToLowerInvariant();

        return normalizedAddress;
    }
}
