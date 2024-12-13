namespace FamilyTreeBlazor.BLL.DTOs;

public class RelationshipDTO(int relationshipId, int personId1, int personId2, RelationshipType relationshipType)
{
    public int Id { get; set; } = relationshipId;
    public int PersonId1 { get; set; } = personId1;
    public int PersonId2 { get; set; } = personId2;
    public RelationshipType RelationshipType { get; set; } = relationshipType;
}
public enum RelationshipType
{
    Parent,
    Spouse
}
