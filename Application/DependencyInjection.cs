using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Common.Interfaces;
using Application.Users.Commands.Create;
using Domain.Entities;
using Application.Meetings.Create;
using Domain.ValueObjects;
using Application.Users.Queries.GetUserMettings;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateUserCommand, User>, CreateUserCommandHandler>();
        services.AddScoped<ICommandHandler<CreateMeetingCommand, Interval>, CreateMeetingCommandHandler>();
        services.AddScoped<IQueryHandler<GetUserMeetingsQuery, IReadOnlyCollection<Meeting>>, GetUserMeetingsQueryHandler>();
    }
}
