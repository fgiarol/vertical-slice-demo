using Demo.Domain.Enums;

namespace Demo.Domain.Entities;

public class Todo : Entity
{
    private readonly List<Step> _steps = [];
    private readonly List<Tag> _tags = [];

    private Todo() { }

    public Todo(string title, ICollection<Step> steps, TodoType type = TodoType.Task, string? description = null, 
        ICollection<Tag>? tags = null)
    {
        Title = title;
        Description = description;
        Type = type;
        IsCompleted = false;
        
        AddSteps(steps);
        
        if (tags is not null) 
            AddTags(tags);
    }

    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsCompleted { get; private set; }
    public TodoType Type { get; private set; }
    public IReadOnlyCollection<Step> Steps => _steps.ToArray();
    public IReadOnlyCollection<Tag> Tags => _tags.ToArray();
    
    public void Update(string title, string? description, TodoType type, bool isCompleted, ICollection<Tag>? tags)
    {
        ChangeTitle(title, description);
        ChangeType(type);
        
        if (isCompleted)
            MarkAsCompleted();

        _tags.Clear();

        if (tags is not null)
            AddTags(tags);

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeTitle(string newTitle, string? description = null)
    {
        Title = newTitle;
        Description = description;
    }
    
    public void ChangeType(TodoType type) => Type = type;

    public void MarkAsCompleted() => IsCompleted = true;

    public void AddSteps(ICollection<Step> steps) => _steps.AddRange(steps);
    public void AddTags(ICollection<Tag> tags) => _tags.AddRange(tags);
}