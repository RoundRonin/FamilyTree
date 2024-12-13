using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FamilyTreeBlazor.DAL.Entities;

namespace FamilyTreeBlazor.DAL.Configurations;

public class PersonEntityTypeConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.BirthDateTime).IsRequired();
        builder.Property(p => p.Sex).IsRequired();

        builder.HasMany(p => p.ChildRelationships)
               .WithOne(r => r.Person1)
               .HasForeignKey(r => r.PersonId1)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.ParentRelationships)
               .WithOne(r => r.Person2)
               .HasForeignKey(r => r.PersonId2)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.SpouseRelationship)
               .WithOne(r => r.Person1)
               .HasForeignKey<Relationship>(r => r.PersonId1)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
