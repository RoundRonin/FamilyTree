using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FamilyTreeBlazor.DAL.Entities;

namespace FamilyTreeBlazor.DAL.Configurations;

public class RelationshipEntityTypeConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(r => r.PersonId1).IsRequired();
        builder.Property(r => r.PersonId2).IsRequired();
        builder.Property(r => r.RelationshipType).IsRequired();
    }
}