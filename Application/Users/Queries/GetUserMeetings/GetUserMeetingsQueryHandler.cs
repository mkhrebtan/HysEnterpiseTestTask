using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Shared;

namespace Application.Users.Queries.GetUserMettings;

public class GetUserMeetingsQueryHandler : IQueryHandler<GetUserMeetingsQuery, IReadOnlyCollection<Meeting>>
{
    private readonly IRepository _repository;

    public GetUserMeetingsQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyCollection<Meeting>>> Handle(GetUserMeetingsQuery query)
    {
        var user = _repository.GetUserById(query.UserId);
        if (user == null) 
        {
            return Result<IReadOnlyCollection<Meeting>>.Failure(new Error("GetUserMeetings.UserNotFound", "User not found."));
        }

        var meetings = _repository.GetUserMeetings(user);
        return Result<IReadOnlyCollection<Meeting>>.Success(meetings);
    }
}
