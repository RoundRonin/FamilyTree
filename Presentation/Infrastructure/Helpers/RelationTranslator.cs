using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

namespace FamilyTreeBlazor.presentation.Infrastructure.Helpers;

public static class RelationTranslator 
{
    public static BLL.DTOs.RelationshipType ToDTOsRelationshipType(Relation relation)
    {
        return relation switch
        {
            Relation.Parent => BLL.DTOs.RelationshipType.Parent,
            Relation.Child => BLL.DTOs.RelationshipType.Parent,
            Relation.Spouse => BLL.DTOs.RelationshipType.Spouse,
            _ => throw new NotImplementedException(),
        };
    }
    public static Models.RelationshipType ToEntitiesRelationshipType(Relation relation)
    {
        return relation switch
        {
            Relation.Parent => Models.RelationshipType.Parent,
            Relation.Child => Models.RelationshipType.Parent,
            Relation.Spouse => Models.RelationshipType.Spouse,
            _ => throw new NotImplementedException(),
        };
    }
}
