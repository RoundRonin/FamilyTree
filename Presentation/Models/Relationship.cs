namespace FamilyTreeBlazor.presentation.Models;

public enum RelationshipType
{
    Parent,
    Spouse
}

public class Relationship(int PersonId1, int PersonId2, RelationshipType RelationshipType, bool FirstIsOld) 
{
    private readonly bool firstIsOld = FirstIsOld;

    public int PersonId1 { get; set; } = PersonId1;
    public int PersonId2 { get; set; } = PersonId2;
    public RelationshipType RelationshipType { get; set; } = RelationshipType;

    public int GetExistingPersonId() { return firstIsOld ? PersonId1 : PersonId2; }
}
