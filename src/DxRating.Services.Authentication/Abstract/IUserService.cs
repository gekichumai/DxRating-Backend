using DxRating.Domain.Entities.Identity;

namespace DxRating.Services.Authentication.Abstract;

public interface IUserService
{
    public Task<User?> GetUserAsync(Guid id);
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<User?> GetUserBySocialLoginAsync(string platform, string identifier);
    public Task<User?> GetUserByWebAuthnAsync(byte[] descriptorId);
    public Task<User?> GetUserByCryptoWalletAsync(string address);

    public Task AddSocialLoginAsync(Guid userId, string platform, string identifier);
    public Task AddWebAuthnAsync(Guid userId, byte[] descriptorId, byte[] publicKey, byte[] userHandle, string credType, Guid aaGuid, uint signCount);
    public Task AddCryptoWallet(Guid userId, string address);

    public Task<User> CreateUserFromLocalAsync(string email, string password);
    public Task<User> CreateUserFromExternalAsync(Guid userId, string email, List<SocialLogin>? socialLogins = null, List<CryptoWallet>? cryptoWallets = null);

    public Task UpdateWebAuthnCounterAsync(byte[] descriptorId, uint signCount);

    public IQueryable<User> GetUserQueryable();
}
