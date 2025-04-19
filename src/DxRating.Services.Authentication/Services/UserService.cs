using DxRating.Database;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Authentication.Utils;
using Microsoft.EntityFrameworkCore;

namespace DxRating.Services.Authentication.Services;

public class UserService : IUserService
{
    private readonly DxDbContext _dbContext;

    public UserService(DxDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region GET

    public async Task<User?> GetUserAsync(Guid id)
    {
        return await GetUserQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await GetUserQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetUserBySocialLoginAsync(string platform, string identifier)
    {
        return await GetUserQueryable()
            .AsNoTracking()
            .Include(x => x.SocialLogins)
            .FirstOrDefaultAsync(x => x.SocialLogins.Any(y => y.Platform == platform && y.Identifier == identifier));
    }

    public async Task<User?> GetUserByWebAuthnAsync(byte[] descriptorId)
    {
        return await GetUserQueryable()
            .AsNoTracking()
            .Include(x => x.WebAuthnDevices)
            .FirstOrDefaultAsync(x => x.WebAuthnDevices.Any(y => y.DescriptorId == descriptorId));
    }

    public async Task<User?> GetUserByCryptoWalletAsync(string address)
    {
        return await GetUserQueryable()
            .AsNoTracking()
            .Include(x => x.CryptoWallets)
            .FirstOrDefaultAsync(x => x.CryptoWallets.Any(y => y.Address == address));
    }

    #endregion

    #region ADD

    public async Task AddSocialLoginAsync(Guid userId, string platform, string identifier)
    {
        var socialLogin = new SocialLogin
        {
            ConnectionId = Guid.NewGuid(),
            Platform = platform,
            Identifier = identifier,
            // Foreign key
            UserId = userId
        };

        await _dbContext.SocialLogins.AddAsync(socialLogin);

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddWebAuthnAsync(Guid userId, byte[] descriptorId, byte[] publicKey, byte[] userHandle, string credType, Guid aaGuid, uint signCount)
    {
        var webAuthnDevice = new WebAuthnDevice
        {
            DescriptorId = descriptorId,
            PublicKey = publicKey,
            UserHandle = userHandle,
            AaGuid = aaGuid,
            SignatureCounter = signCount,
            CredType = credType,
            // Foreign key
            UserId = userId
        };

        await _dbContext.WebAuthnDevices.AddAsync(webAuthnDevice);

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCryptoWallet(Guid userId, string address)
    {
        var cryptoWallet = new CryptoWallet
        {
            Address = address,
            // Foreign key
            UserId = userId
        };

        await _dbContext.CryptoWallets.AddAsync(cryptoWallet);

        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region CREATE

    public async Task<User> CreateUserFromLocalAsync(string email, string password)
    {
        var hashedPassword = SecurityUtils.HashPassword(password);

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = email,
            Password = hashedPassword,
            EmailConfirmed = false
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User> CreateUserFromExternalAsync(
        Guid userId, string email,
        List<SocialLogin>? socialLogins = null,
        List<CryptoWallet>? cryptoWallets = null)
    {
        var user = new User
        {
            UserId = userId,
            Email = email,
            EmailConfirmed = true,
            SocialLogins = socialLogins ?? [],
            CryptoWallets = cryptoWallets ?? []
        };

        await _dbContext.Users.AddAsync(user);

        await _dbContext.SaveChangesAsync();

        return user;
    }

    #endregion

    public async Task UpdateWebAuthnCounterAsync(byte[] descriptorId, uint signCount)
    {
        var webAuthn = await _dbContext.WebAuthnDevices
                           .FirstOrDefaultAsync(x => x.DescriptorId == descriptorId)
                       ?? throw new InvalidOperationException("Device not found");

        webAuthn.SignatureCounter = signCount;

        await _dbContext.SaveChangesAsync();
    }

    public IQueryable<User> GetUserQueryable()
    {
        var queryable = _dbContext.Users.AsQueryable();
        return queryable;
    }
}
