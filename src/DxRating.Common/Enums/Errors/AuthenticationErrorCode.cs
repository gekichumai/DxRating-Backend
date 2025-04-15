using System.Diagnostics.CodeAnalysis;

namespace DxRating.Common.Enums.Errors;

// ReSharper disable InconsistentNaming

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public enum AuthenticationErrorCode
{
    // Local Authentication
    EMAIL_ALREADY_EXISTS = 1000,
    PASSWORD_LOW_COMPLEXITY = 1001,
}
