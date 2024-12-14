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
    public static Entities.RelationshipType ToEntitiesRelationshipType(Relation relation)
    {
        return relation switch
        {
            Relation.Parent => Entities.RelationshipType.Parent,
            Relation.Child => Entities.RelationshipType.Parent,
            Relation.Spouse => Entities.RelationshipType.Spouse,
            _ => throw new NotImplementedException(),
        };
    }
}
