using DxRating.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DxRating.Database.Configurator;

public class SessionConfigurator : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.AccessToken);
        builder.HasIndex(x => x.RefreshToken);
        builder.HasIndex(x => x.AccessTokenExpiresAt);
        builder.HasIndex(x => x.RefreshTokenExpiresAt);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}
