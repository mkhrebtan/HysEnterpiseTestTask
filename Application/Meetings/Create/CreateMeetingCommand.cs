using Application.Common.Interfaces;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Meetings.Create;

public sealed record CreateMeetingCommand : ICommand<Interval>
{
    public IReadOnlyCollection<Guid> ParticipantIds { get; init; } = new List<Guid>();

    public int DurationInMinutes { get; init; } = 60;

    public DateTime EarliestStart { get; init; } = DateTime.UtcNow;

    public DateTime LatestEnd { get; init; } = DateTime.UtcNow.AddHours(8);
}