using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared;

public interface IRepository
{
    List<Meeting> GetUserMeetings(User user);

    User? GetUserById(Guid id);
}
