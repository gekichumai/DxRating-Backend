namespace DxRating.Services.Authentication.Utils;

public class SecurityUtils
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 10);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public static bool VerifyPasswordComplexity(string password)
    {
        // 1. Only accept ASCII characters
        // 2. No spaces
        // 3. At least 8 characters long
        // 4. Require at least 3 of:
        //   a. Numbers
        //   b. UPPER CASE CHARACTERS
        //   c. lower case characters
        //   d. Symbols

        // Verify 1 & 2
        if (password.Any(c => c is < (char)33 or > (char)126))
        {
            return false;
        }

        // Verify 3
        if (password.Length < 8)
        {
            return false;
        }

        // Verify 4
        var upperCaseCount = password.Count(c => c is >= 'A' and <= 'Z');
        var lowerCaseCount = password.Count(c => c is >= 'a' and <= 'z');
        var numberCount = password.Count(c => c is >= '0' and <= '9');
        var symbolCount = password.Length - upperCaseCount - lowerCaseCount - numberCount;

        var typeCount = (upperCaseCount > 0 ? 1 : 0) +
                        (lowerCaseCount > 0 ? 1 : 0) +
                        (numberCount > 0 ? 1 : 0) +
                        (symbolCount > 0 ? 1 : 0);

        return typeCount >= 3;
    }
}
