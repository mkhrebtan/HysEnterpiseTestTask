using Domain.Entities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Persistence;

public class Repository : IRepository
{
    public List<Meeting> Meetings { get; private set; }

    public List<User> Users { get; private set; }

    public Repository()
    {
        Users = GetAllUsers();
        Meetings = GetAllMeetings();
    }

    private List<User> GetAllUsers()
    {
        return new List<User>
        {
            new User(new Guid("11111111-1111-1111-1111-111111111111"), "Alice"),
            new User(new Guid("22222222-2222-2222-2222-222222222222"), "Bob"),
            new User(new Guid("33333333-3333-3333-3333-333333333333"), "Charlie"),
            new User(new Guid("44444444-4444-4444-4444-444444444444"), "Diana"),
            new User(new Guid("55555555-5555-5555-5555-555555555555"), "Eve")
        };
    }

    private List<Meeting> GetAllMeetings()
    {
        var meetings = new List<Meeting>
        {
            new Meeting(new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Interval(new DateTime(2025, 06, 20, 9, 0, 0), new DateTime(2025, 06, 20, 9, 30, 0))),
            new Meeting(new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Interval(new DateTime(2025, 06, 20, 10, 0, 0), new DateTime(2025, 06, 20, 10, 30, 0))),
            new Meeting(new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Interval(new DateTime(2025, 06, 20, 10, 30, 0), new DateTime(2025, 06, 20, 11, 30, 0))),
            new Meeting(new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Interval(new DateTime(2025, 06, 20, 12, 0, 0), new DateTime(2025, 06, 20, 16, 30, 0))),
        };

        meetings[0].AddParticipant(Users[0]);
        meetings[0].AddParticipant(Users[1]);
        meetings[1].AddParticipant(Users[2]);
        meetings[1].AddParticipant(Users[4]);
        meetings[1].AddParticipant(Users[0]);
        meetings[2].AddParticipant(Users[3]);
        meetings[2].AddParticipant(Users[1]);
        meetings[3].AddParticipant(Users[2]);
        meetings[3].AddParticipant(Users[4]);
        meetings[3].AddParticipant(Users[3]);

        return meetings;
    }

    public User? GetUserById(Guid id)
    {
        return Users.FirstOrDefault(u => u.Id == id);
    }

    public Meeting? GetMeetingById(Guid id)
    {
        return Meetings.FirstOrDefault(m => m.Id == id);
    }

    public List<Meeting> GetUserMeetings(User user)
    {
        return Meetings
            .Where(m => m.Participants.Contains(user))
            .ToList();
    }

    public void AddUser(User user)
    {
        if (!Users.Any(u => u.Id == user.Id))
        {
            Users.Add(user);
        }
    }

    public void AddMeeting(Meeting meeting)
    {
        if (!Meetings.Any(m => m.Id == meeting.Id))
        {
            Meetings.Add(meeting);
        }
    }
}
