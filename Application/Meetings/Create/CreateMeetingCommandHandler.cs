using Application.Common.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Meetings.Create;

public class CreateMeetingCommandHandler : ICommandHandler<CreateMeetingCommand, Interval>
{
    private readonly IRepository _repository;

    public CreateMeetingCommandHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Interval>> Handle(CreateMeetingCommand command)
    {
        var users = command.ParticipantIds
            .Select(id => _repository.GetUserById(id))
            .Where(user => user != null)
            .ToList();

        if (users.Count != command.ParticipantIds.Count)
        {
            return Result<Interval>.Failure(new Error("CreateMeeting.InvalidParticipants", "Some participants do not exist."));
        }

        var meetings = users
            .Select(user => _repository.GetUserMeetings(user!))
            .SelectMany(meetings => meetings)
            .Distinct()
            .Where(m => m.Interval.End > command.EarliestStart && m.Interval.Start < command.LatestEnd)
            .ToList();

        var intervals = meetings.Select(m => m.Interval).ToList();

        var mergedIntervals = MergeIntervals(intervals);

        var earliestSlot = FindFirstFreeSlot(
            mergedIntervals,
            command.EarliestStart,
            command.LatestEnd,
            TimeSpan.FromMinutes(command.DurationInMinutes));

        if (earliestSlot == null)
        {
            return Result<Interval>.Failure(new Error("CreateMeeting.NoAvailableSlot", "No available slot found for the meeting."));
        }

        return await Task.FromResult(
            Result<Interval>.Success(
                new Interval(earliestSlot.Value, earliestSlot.Value.AddMinutes(command.DurationInMinutes))
            )
        );
    }

    private static List<Interval> MergeIntervals(List<Interval> intervals)
    {
        if (!intervals.Any())
        {
            return new List<Interval>();
        }

        var sortedIntervals = intervals
            .OrderBy(i => i.Start)
            .ThenBy(i => i.End)
            .ToList();

        var merged = new List<Interval> { sortedIntervals[0] };
        foreach (var current in sortedIntervals.Skip(1))
        {
            var last = merged.Last();
            if (current.Start <= last.End)
            {
                if (current.End > last.End)
                {
                    merged[^1] = new Interval(last.Start, current.End);
                }
            }
            else
            {
                merged.Add(current);
            }
        }

        return merged;
    }

    private static DateTime? FindFirstFreeSlot(
        List<Interval> intervals,
        DateTime earliest,
        DateTime latest,
        TimeSpan duration)
    {
        var cursor = earliest;

        foreach (var block in intervals)
        {
            if (cursor + duration <= block.Start)
                return cursor;

            if (cursor < block.End)
                cursor = block.End;
        }

        return cursor + duration <= latest
            ? cursor
            : (DateTime?)null;
    }
}
