using FamilyTreeBlazor.DAL.Infrastructure;

namespace FamilyTreeBlazor.DAL.Entities;

public class Relationship(int PersonId1, int PersonId2, RelationshipType RelationshipType): IEntity
{
    public int Id { get; set; }
    public int PersonId1 { get; set; } = PersonId1;
    public int PersonId2 { get; set; } = PersonId2;
    public RelationshipType RelationshipType { get; set; } = RelationshipType;
}

public enum RelationshipType
{
    Parent,
    Spouse
}
