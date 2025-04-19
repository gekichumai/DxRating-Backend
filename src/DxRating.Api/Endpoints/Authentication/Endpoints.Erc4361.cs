using System.Text.Json;
using System.Text.Json.Serialization;
using DxRating.Api.Endpoints.Authentication.Dto;
using DxRating.Common.Enums;
using DxRating.Common.Extensions;
using DxRating.Common.Options;
using DxRating.Common.Utils;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Api.Extensions;
using DxRating.Services.Api.Models;
using DxRating.Services.Api.Services;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Authentication.Constants;
using DxRating.Services.Authentication.Models;
using DxRating.Services.Authentication.Options;
using DxRating.Services.Authentication.Services;
using DxRating.Services.Authentication.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Nethereum.Signer;

namespace DxRating.Api.Endpoints.Authentication;

public partial class Endpoints
{
    [EndpointDescription("Get Erc4361 challenge message")]
    private static async Task<Ok<Erc4361ChallengeDto>> GetErc4361Challenge(
        [FromServices] IDistributedCache distributedCache,
        [FromServices] IConfiguration configuration,
        [FromQuery(Name = "address")] string walletAddress)
    {
        var options = configuration.GetOptions<Erc4361Options>("Authentication:Erc4361");

        var address = EthereumUtils.NormalizeAddress(walletAddress);
        var now = DateTimeOffset.UtcNow;
        var expireAt = now.AddMinutes(5);

        var challengeId = Guid.NewGuid();
        var nonce = RandomUtils.GetRandomBase64String(16);

        var erc4361 = new Erc4361Model
        {
            ChallengeId = challengeId,
            Nonce = nonce,
            Address = address,
            FullyQualifiedDomainName = options.FullyQualifiedDomainName,
            Uri = options.Uri,
            IssueAt = now,
            ExpirationTime = expireAt
        };

        var message = erc4361.ToMessage();

        var cacheKey = GetErc4361ChallengeCacheKey(challengeId);
        var cacheString = Erc4361ChallengeCache.MakeCache(address, message);
        await distributedCache.SetStringAsync(cacheKey, cacheString, new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = expireAt
        });

        return TypedResults.Ok(new Erc4361ChallengeDto
        {
            ChallengeId = challengeId,
            Message = message
        });
    }

    [EndpointDescription("Verify Erc4361 signature")]
    private static async Task<Results<Ok<UserTokenDto>, BadRequest<ErrorResponse>>> VerifyErc4361SignatureAsync(
        [FromBody] Erc4361SignatureDto dto,
        [FromServices] IDistributedCache distributedCache,
        [FromServices] IUserService userService,
        [FromServices] SessionService sessionService,
        [FromServices] IConfiguration configuration,
        [FromServices] CurrentUser currentUser)
    {
        var cacheKey = GetErc4361ChallengeCacheKey(dto.ChallengeId);
        var cacheString = await distributedCache.GetStringAsync(cacheKey);
        if (string.IsNullOrEmpty(cacheString))
        {
            return ErrorCode.InvalidErc4361ChallengeId.ToResponse().ToBadRequest();
        }
        var cache = Erc4361ChallengeCache.ParseCache(cacheString);
        var address = cache.Address;

        var signer = new EthereumMessageSigner();

        try
        {
            var extractedAddress = signer.EncodeUTF8AndEcRecover(cache.Message, dto.Signature);
            var normalizedAddress = EthereumUtils.NormalizeAddress(extractedAddress);
            if (string.Equals(normalizedAddress, address, StringComparison.OrdinalIgnoreCase) is false)
            {
                return ErrorCode.InvalidErc4361SignatureSigner.ToResponse().ToBadRequest();
            }
        }
        catch (Exception)
        {
            return ErrorCode.InvalidErc4361Signature.ToResponse().ToBadRequest();
        }

        // Try to get user by using crypto wallet
        {
            var user = await userService.GetUserByCryptoWalletAsync(address);
            if (user is not null)
            {
                var session = await sessionService.CreateSessionAsync(user, currentUser.UserAgent, currentUser.IpAddress);

                return session.MapToUserTokenDto().ToOk();
            }
        }

        // Try to authenticate the user using Bearer token
        {
            await currentUser.AuthenticateAsync(AuthenticationConstants.BearerAuthenticationScheme);
            var user = await currentUser.GetUserAsync();
            if (currentUser.IsAuthenticated && user is not null)
            {
                await userService.AddCryptoWallet(user.UserId, address);

                var session = await sessionService.CreateSessionAsync(user, currentUser.UserAgent, currentUser.IpAddress);

                return session.MapToUserTokenDto().ToOk();
            }
        }

        // Create a new user
        {
            var serverOptions = configuration.GetOptions<ServerOptions>("Server");
            var userId = Guid.NewGuid();
            var email = $"wallet_{address}@{serverOptions.DefaultEmailDomain}";
            var user = await userService.CreateUserFromExternalAsync(userId, email,
                cryptoWallets: [
                    new CryptoWallet
                    {
                        Address = address
                    }
                ]);

            var session = await sessionService.CreateSessionAsync(user, currentUser.UserAgent, currentUser.IpAddress);

            return session.MapToUserTokenDto().ToOk();
        }
    }

    private static string GetErc4361ChallengeCacheKey(Guid id)
    {
        return $"auth-demo:erc4361:challenge:{id}";
    }

    private sealed record Erc4361ChallengeCache
    {
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        public static string MakeCache(string address, string message)
        {
            return JsonSerializer.Serialize(new Erc4361ChallengeCache
            {
                Address = address,
                Message = message
            });
        }

        public static Erc4361ChallengeCache ParseCache(string cacheString)
        {
            return JsonSerializer.Deserialize<Erc4361ChallengeCache>(cacheString) ?? throw new InvalidOperationException("Invalid cache string");
        }
    }
}
