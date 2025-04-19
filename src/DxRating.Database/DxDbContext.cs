using DxRating.Common.Models.Data.Enums;
using DxRating.Database.Converter;
using DxRating.Domain.Entities.Identity;
using DxRating.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DxRating.Database;

public class DxDbContext(DbContextOptions<DxDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DxDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<TokenType>()
            .HaveConversion<EnumToStringConverter<TokenType>>();

        configurationBuilder.Properties<DxCategoryType>()
            .HaveConversion<DxCategoryTypeConverter>();

        configurationBuilder.Properties<DxDifficultyType>()
            .HaveConversion<DxDifficultyTypeConverter>();

        configurationBuilder.Properties<DxSheetType>()
            .HaveConversion<DxSheetTypeConverter>();

        configurationBuilder.Properties<DxVersionType>()
            .HaveConversion<DxVersionTypeConverter>();
    }

    #region Identities

    public DbSet<CryptoWallet> CryptoWallets => Set<CryptoWallet>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SocialLogin> SocialLogins => Set<SocialLogin>();
    public DbSet<Token> Tokens => Set<Token>();
    public DbSet<Totp> Totps => Set<Totp>();
    public DbSet<User> Users => Set<User>();
    public DbSet<WebAuthnDevice> WebAuthnDevices => Set<WebAuthnDevice>();

    #endregion
}
