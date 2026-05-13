namespace Demo.Domain.Entities;

public class Step : Entity
{
    private Step() { }
    
    public Step(string title, bool isCompleted, int order)
    {
        Title = title;
        IsCompleted = isCompleted;
        Order = order;
    }

    public string Title { get; private set; } = null!;
    public bool IsCompleted { get; private set; }
    public int Order { get; set; }

    public void Update(string title, bool isCompleted, int order)
    {
        ChangeTitle(title);
        UpdateOrder(order);

        if (isCompleted)
            MarkAsCompleted();
        else
            MarkAsIncomplete();
    }
    
    public void ChangeTitle(string newTitle) => Title = newTitle;
    public void UpdateOrder(int newOrder) => Order = newOrder;
    public void MarkAsCompleted() => IsCompleted = true;
    public void MarkAsIncomplete() => IsCompleted = false;
}