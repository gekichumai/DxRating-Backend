namespace DxRating.Common.Enums;

public enum ErrorCode
{
    #region Generic

    Unknown,

    #endregion

    #region Turnstile

    TurnstileServiceUnavailable,
    TurnstileVerificationFailed,

    #endregion

    #region Authentication

    // Auth
    EmailAlreadyInUse,
    PasswordLowComplexity,
    InvalidCredentials,

    // Session
    SessionExpired,
    RefreshTokenExpired,

    // User
    UserNotFound,
    EmailNotConfirmed

    #endregion
}
