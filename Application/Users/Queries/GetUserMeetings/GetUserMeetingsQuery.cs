using Application.Common.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetUserMettings;

public sealed record GetUserMeetingsQuery : IQuery<IReadOnlyCollection<Meeting>>
{
    required public Guid UserId { get; init; }
}