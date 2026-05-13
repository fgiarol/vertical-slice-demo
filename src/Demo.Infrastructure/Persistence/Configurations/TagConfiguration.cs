using Demo.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations;

public class TagConfiguration : EntityConfiguration<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.Name).HasMaxLength(500).IsRequired();
        builder.HasIndex(t => t.Name).IsUnique();
    }
}