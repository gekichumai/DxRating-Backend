using DxRating.Common.Models.Data.Enums;
using DxRating.Database.Converter;
using DxRating.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

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

    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SocialLogin> SocialLogins => Set<SocialLogin>();
    public DbSet<Token> Tokens => Set<Token>();
    public DbSet<User> Users => Set<User>();

    #endregion
}
