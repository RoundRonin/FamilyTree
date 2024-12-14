using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;
using System.IO;

namespace FamilyTreeBlazor.DAL.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var connectionString = DbContextConfigurationHelper.BuildConnectionString();

            // Configure DbContextOptionsBuilder
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            DbContextConfigurationHelper.Configure(optionsBuilder, connectionString);

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
