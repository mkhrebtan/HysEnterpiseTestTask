using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Meeting : Entity
{
    private readonly HashSet<User> _participants = new HashSet<User>();

    public Meeting(Interval interval)
        : this(Guid.NewGuid(), interval)
    {
    }

    public Meeting(Guid id, Interval interval)
        : base(id)
    {
        Interval = interval;
    }

    public IReadOnlyCollection<User> Participants => _participants;

    public Interval Interval { get; private set; }

    public Result AddParticipant(User user)
    {
        if (user == null)
        {
            return Result.Failure(new Error("AddParticipant.NullUser", "Null user provided."));
        }
        if (_participants.Contains(user))
        {
            return Result.Failure(new Error("AddParticipant.UserExist", "User is already a participant"));
        }

        _participants.Add(user);
        return Result.Success();
    }
}
