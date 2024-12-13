using FamilyTreeBlazor.DAL.Infrastructure;

namespace FamilyTreeBlazor.DAL.Entities;

public class Person : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime BirthDateTime { get; set; }
    public bool Sex { get; set; }

    // Navigation properties
    public ICollection<Relationship> ParentRelationships { get; set; }
    public ICollection<Relationship> ChildRelationships { get; set; }
    public Relationship SpouseRelationship { get; set; }
}
