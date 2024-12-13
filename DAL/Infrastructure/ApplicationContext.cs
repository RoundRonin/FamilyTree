using Microsoft.EntityFrameworkCore;
using FamilyTreeBlazor.DAL.Entities;
using FamilyTreeBlazor.DAL.Configurations;

namespace FamilyTreeBlazor.DAL.Infrastructure;

public class FamilyTreeContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Relationship> Relationships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PersonEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RelationshipEntityTypeConfiguration());
    }
}

