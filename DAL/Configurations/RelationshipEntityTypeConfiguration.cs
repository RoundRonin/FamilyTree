using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FamilyTreeBlazor.DAL.Entities;

namespace FamilyTreeBlazor.DAL.Configurations;

public class RelationshipEntityTypeConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.PersonId1).IsRequired();
        builder.Property(r => r.PersonId2).IsRequired();
        builder.Property(r => r.RelationshipType).IsRequired();

        builder.HasOne(r => r.Person1)
               .WithMany(p => p.ChildRelationships)
               .HasForeignKey(r => r.PersonId1)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Person2)
               .WithMany(p => p.ParentRelationships)
               .HasForeignKey(r => r.PersonId2)
               .OnDelete(DeleteBehavior.Restrict);
    }
}