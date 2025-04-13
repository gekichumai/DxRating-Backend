using DxRating.Common.Models.Data.Enums;
using DxRating.Database.Converter;
using Microsoft.EntityFrameworkCore;

namespace DxRating.Database;

public class DxDbContext(DbContextOptions<DxDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSnakeCaseNamingConvention();
    }

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
}
