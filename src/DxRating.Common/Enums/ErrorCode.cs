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

    // Local
    EmailAlreadyInUse,
    PasswordLowComplexity,
    InvalidCredentials,

    // WebAuthn
    InvalidAttestationId,
    InvalidAssertionId,
    InvalidDescriptorId,

    // Erc4361
    InvalidErc4361ChallengeId,
    InvalidErc4361SignatureSigner,
    InvalidErc4361Signature,

    // Session
    SessionExpired,
    RefreshTokenExpired,
    InvalidAuthenticationScheme,

    // User
    UserNotFound,
    EmailNotConfirmed

    #endregion
}
