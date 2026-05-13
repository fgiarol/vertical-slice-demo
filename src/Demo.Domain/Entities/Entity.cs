namespace Demo.Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedAt { get; init; } =  DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; protected set; }
}