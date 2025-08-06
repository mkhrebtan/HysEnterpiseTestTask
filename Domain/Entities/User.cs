namespace Domain.Entities;

public class User : Entity
{
    public User(string name)
        : this(Guid.NewGuid(), name) 
    {
    }

    public User(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    public string Name { get; private set; }
}