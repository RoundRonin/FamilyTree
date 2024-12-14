using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FamilyTreeBlazor.DAL.Entities;

namespace FamilyTreeBlazor.DAL.Configurations;

public class PersonEntityTypeConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd(); 
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.BirthDateTime).IsRequired();
        builder.Property(p => p.Sex).IsRequired();
    }
}
