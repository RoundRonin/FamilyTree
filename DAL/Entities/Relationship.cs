using FamilyTreeBlazor.DAL.Infrastructure;

namespace FamilyTreeBlazor.DAL.Entities;

public class Relationship : IEntity
{
    public int Id { get; set; }
    public int PersonId1 { get; set; }
    public int PersonId2 { get; set; }
    public RelationshipType RelationshipType { get; set; }

    // Navigation properties
    public required Person Person1 { get; set; }
    public required Person Person2 { get; set; }
}

public enum RelationshipType
{
    Parent,
    Spouse
}
