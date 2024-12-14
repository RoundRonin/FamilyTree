using Microsoft.EntityFrameworkCore;
using FamilyTreeBlazor.DAL.Entities;
using FamilyTreeBlazor.DAL.Configurations;

namespace FamilyTreeBlazor.DAL.Infrastructure;
public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Relationship> Relationships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PersonEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RelationshipEntityTypeConfiguration());
    }
}

