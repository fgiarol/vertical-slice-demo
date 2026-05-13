using Demo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations;

public class TodoConfiguration : EntityConfiguration<Todo>
{
    public override void Configure(EntityTypeBuilder<Todo> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.Title).HasMaxLength(500).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(1000).IsRequired(false);
        builder.Property(t => t.IsCompleted).IsRequired();
        builder.Property(t => t.Type).HasConversion<string>().IsRequired();
        
        builder.HasMany(t => t.Tags)
            .WithMany()
            .UsingEntity("TagTodo",
                r => r.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId"),
                l => l.HasOne(typeof(Todo)).WithMany().HasForeignKey("TodoId"),
                j =>
                {
                    j.HasKey("TagsId", "TodoId");
                    j.ToTable("TodoTags");
                });

        builder.HasMany(t => t.Steps)
            .WithOne()
            .HasForeignKey("TodoId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(t => t.Steps)
            .HasField("_steps")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(t => t.Tags)
            .HasField("_tags")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}