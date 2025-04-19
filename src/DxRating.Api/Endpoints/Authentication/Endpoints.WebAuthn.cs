using DxRating.Api.Endpoints.Authentication.Dto;
using DxRating.Common.Enums;
using DxRating.Database;
using DxRating.Services.Api.Extensions;
using DxRating.Services.Api.Models;
using DxRating.Services.Api.Services;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Authentication.Services;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace DxRating.Api.Endpoints.Authentication;

public partial class Endpoints
{
    [EndpointDescription("WebAuthn registration")]
    private static async Task<Results<Ok<WebAuthnAttestationDto>, BadRequest<ErrorResponse>>> AttestationAsync(
        [FromServices] IFido2 fido2,
        [FromServices] CurrentUser currentUser,
        [FromServices] IUserService userService,
        [FromServices] IDistributedCache distributedCache)
    {
        var user = await userService.GetUserQueryable()
                .Include(x => x.WebAuthnDevices)
                .FirstOrDefaultAsync(x => x.UserId == currentUser.UserId)
                   ?? throw new InvalidOperationException("User not found");

        var existingCredentials = user.WebAuthnDevices
            .Select(x => new PublicKeyCredentialDescriptor(x.DescriptorId))
            .ToList();

        var name = user.Email;

        var fido2User = new Fido2User
        {
            Id = user.UserId.ToByteArray(),
            Name = name,
            DisplayName = name
        };

        var options = fido2.RequestNewCredential(new RequestNewCredentialParams
        {
            User = fido2User,
            ExcludeCredentials = existingCredentials,
            AuthenticatorSelection = AuthenticatorSelection.Default,
            AttestationPreference = AttestationConveyancePreference.None,
            Extensions = new AuthenticationExtensionsClientInputs
            {
                Extensions = true,
                CredProps = true
            }
        });

        var expireAt = DateTimeOffset.UtcNow.AddMinutes(5);

        var attestationId = Guid.NewGuid();
        var cacheKey = GetAttestationChallengeCacheKey(attestationId);
        await distributedCache.SetStringAsync(cacheKey, options.ToJson(), new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = expireAt
        });

        return TypedResults.Ok(new WebAuthnAttestationDto
        {
            ChallengeId = attestationId,
            Options = options
        });
    }

    [EndpointDescription("WebAuthn registration verification")]
    private static async Task<Results<EmptyHttpResult, BadRequest<ErrorResponse>>> AttestationVerifyAsync(
        [FromBody] WebAuthnAttestationVerifyDto dto,
        [FromServices] IFido2 fido2,
        [FromServices] IDistributedCache distributedCache,
        [FromServices] DxDbContext dbContext,
        [FromServices] IUserService userService)
    {
        // Find the attestation
        var cacheKey = GetAttestationChallengeCacheKey(dto.ChallengeId);
        var options = await distributedCache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(options))
        {
            return ErrorCode.InvalidAttestationId.ToResponse().ToBadRequest();
        }

        // Build CredentialCreateOptions
        var credentialCreateOptions = CredentialCreateOptions.FromJson(options);

        // Verify and make the credentials
        var credential = await fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
        {
            AttestationResponse = dto.AttestationResponse,
            OriginalOptions = credentialCreateOptions,
            IsCredentialIdUniqueToUserCallback = async (p, token) =>
            {
                var exist = await dbContext.WebAuthnDevices.AnyAsync(x => x.DescriptorId == p.CredentialId, token);
                return !exist;
            }
        });

        var userId = new Guid(credential.User.Id);

        await userService.AddWebAuthnAsync(userId,
            credential.Id, credential.PublicKey, credential.User.Id,
            credential.Type.ToString(), credential.AaGuid, credential.SignCount);

        // Return OK
        return TypedResults.Empty;
    }

    [EndpointDescription("WebAuthn assertion")]
    private static async Task<Ok<WebAuthnAssertionDto>> AssertionAsync(
        [FromServices] IFido2 fido2,
        [FromServices] IDistributedCache distributedCache)
    {
        var options = fido2.GetAssertionOptions(new GetAssertionOptionsParams
        {
            AllowedCredentials = [],
            UserVerification = UserVerificationRequirement.Required
        });

        var challengeId = Guid.NewGuid();
        var cacheKey = GetAssertionChallengeCacheKey(challengeId);

        await distributedCache.SetStringAsync(cacheKey, options.ToJson());

        var dto = new WebAuthnAssertionDto
        {
            Options = options,
            ChallengeId = challengeId
        };

        return TypedResults.Ok(dto);
    }

    [EndpointDescription("WebAuthn assertion verification")]
    private static async Task<Results<Ok<UserTokenDto>, BadRequest<ErrorResponse>>> AssertionVerifyAsync(
        [FromBody] WebAuthnAssertionVerifyDto dto,
        [FromServices] IUserService userService,
        [FromServices] IFido2 fido2,
        [FromServices] IDistributedCache distributedCache,
        [FromServices] SessionService sessionService,
        [FromServices] ICurrentUser currentUser)
    {
        // Find the challenge
        var cacheKey = GetAssertionChallengeCacheKey(dto.ChallengeId);
        var options = await distributedCache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(options))
        {
            return ErrorCode.InvalidAssertionId.ToResponse().ToBadRequest();
        }

        // Build AssertionOptions
        var assertionOptions = AssertionOptions.FromJson(options);

        // Find stored credential
        var user = await userService.GetUserByWebAuthnAsync(dto.AssertionResponse.Id);
        if (user is null)
        {
            return ErrorCode.InvalidDescriptorId.ToResponse().ToBadRequest();
        }
        var storedCredential = user.WebAuthnDevices
            .First(x => x.DescriptorId.SequenceEqual(dto.AssertionResponse.Id));

        // Verify the assertion
        var verifyAssertionResult = await fido2.MakeAssertionAsync(new MakeAssertionParams
        {
            AssertionResponse = dto.AssertionResponse,
            OriginalOptions = assertionOptions,
            StoredPublicKey = storedCredential.PublicKey,
            StoredSignatureCounter = 0,
            IsUserHandleOwnerOfCredentialIdCallback =  (p, _) =>
                Task.FromResult(p.CredentialId.SequenceEqual(storedCredential.DescriptorId) &&
                                p.UserHandle.SequenceEqual(storedCredential.UserHandle))
        });

        await userService.UpdateWebAuthnCounterAsync(verifyAssertionResult.CredentialId, verifyAssertionResult.SignCount);

        // Return the token
        var session = await sessionService.CreateSessionAsync(user, currentUser.UserAgent, currentUser.IpAddress);

        return session.MapToUserTokenDto().ToOk();
    }

    private static string GetAttestationChallengeCacheKey(Guid id)
    {
        return $"auth-demo:webauthn:attestation:{id}";
    }

    private static string GetAssertionChallengeCacheKey(Guid id)
    {
        return $"auth-demo:webauthn:assertion:{id}";
    }
}
