using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Users.Commands.Create;

public sealed record CreateUserCommand : ICommand<User>
{
    required public string Name { get; init; }
}
