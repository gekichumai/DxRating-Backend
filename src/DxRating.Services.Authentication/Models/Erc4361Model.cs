namespace DxRating.Services.Authentication.Models;

public record Erc4361Model
{
    public required Guid ChallengeId { get; init; }
    public required string Nonce { get; init; }
    public required string Address { get; init; }
    public required string FullyQualifiedDomainName { get; init; }
    public required string Uri { get; init; }
    public required DateTimeOffset IssueAt { get; init; }
    public required DateTimeOffset ExpirationTime { get; init; }

    public string ToMessage()
    {
        // https://eips.ethereum.org/EIPS/eip-4361#abnf-message-format

        var message = $"""
                       {FullyQualifiedDomainName} wants you to sign in with your Ethereum account:
                       {Address}

                       I am the owner of this address and I want to sign in.

                       URI: {Uri}
                       Version: 1
                       Chain ID: 1
                       Nonce: {Nonce}
                       Issue At: {IssueAt:O}
                       Expiration Time: {ExpirationTime:O}
                       Request ID: {ChallengeId}
                       """;

        // Make sure line ending is LF
        return message.Replace("\r\n", "\n");
    }
}
