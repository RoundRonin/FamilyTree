namespace FamilyTreeBlazor.presentation.Models;

public class RelationLink
{
    public int Source { get; set; }
    public int Target { get; set; }
    public RelationshipType RelationshipType { get; set; }
}

