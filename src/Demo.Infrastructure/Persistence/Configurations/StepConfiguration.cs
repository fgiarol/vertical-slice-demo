using Demo.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations;

public class StepConfiguration : EntityConfiguration<Step>
{
    public override void Configure(EntityTypeBuilder<Step> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Title).HasMaxLength(500).IsRequired();
        builder.Property(s => s.Order).IsRequired();
        builder.Property(s => s.IsCompleted).IsRequired();
    }
}