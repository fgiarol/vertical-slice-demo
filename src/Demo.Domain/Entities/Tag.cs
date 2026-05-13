namespace Demo.Domain.Entities;

public class Tag : Entity
{
    private Tag() { }
    
    public Tag(string name)
    {
        Name = name;
    }

    public string Name { get; private set; } = null!;

    public void ChangeName(string newName) => Name = newName;
}