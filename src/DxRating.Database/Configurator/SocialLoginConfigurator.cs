using DxRating.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DxRating.Database.Configurator;

public class SocialLoginConfigurator : IEntityTypeConfiguration<SocialLogin>
{
    public void Configure(EntityTypeBuilder<SocialLogin> builder)
    {
        builder.HasIndex(x => new
        {
            x.Platform,
            x.Identifier
        });
    }
}
