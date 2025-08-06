using Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistence(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository), typeof(Repository));
    }
}
