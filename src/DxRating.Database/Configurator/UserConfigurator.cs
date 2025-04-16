using DxRating.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DxRating.Database.Configurator;

public class UserConfigurator : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.Email);
        builder.HasMany(x => x.SocialLogins)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}
