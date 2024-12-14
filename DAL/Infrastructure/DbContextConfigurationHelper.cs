using Microsoft.EntityFrameworkCore;

namespace FamilyTreeBlazor.DAL.Infrastructure;

public static class DbContextConfigurationHelper
{
    public static void Configure(DbContextOptionsBuilder<ApplicationContext> optionsBuilder, string connectionString)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }

    public static string BuildConnectionString()
    {
        // Load environment variables from .env file
        var rootEnvPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, ".env");
        DotNetEnv.Env.Load(rootEnvPath);

        // Construct the connection string from environment variables
        return $"Host={Environment.GetEnvironmentVariable("HOST")};" +
               $"Port={Environment.GetEnvironmentVariable("PORT")};" +
               $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
               $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
               $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";
    }
}
