using Microsoft.Extensions.DependencyInjection;

namespace FamilyTreeBlazor.DAL.Infrastructure;

public class DbContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ApplicationContext CreateDbContext()
    {
        return _serviceProvider.GetRequiredService<ApplicationContext>();
    }
}
