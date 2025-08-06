using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Shared;

namespace Application.Users.Commands.Create;

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, User>
{
    public async Task<Result<User>> Handle(CreateUserCommand command)
    {
        User user = new(command.Name);
        return await Task.FromResult(Result<User>.Success(user));
    }
}
