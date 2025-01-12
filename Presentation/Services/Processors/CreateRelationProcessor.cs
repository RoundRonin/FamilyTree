using FamilyTreeBlazor.presentation.Infrastructure.Helpers;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;

namespace FamilyTreeBlazor.presentation.Services.Processors;

public class CreateRelationProcessor(IPersonRelationshipService personRelationshipService)
{
    private readonly IPersonRelationshipService _personRelationshipService = personRelationshipService;

    public void CreateRelation(int editId, int targetId, Relation relation)
    {
        var relationshipType = RelationTranslator.ToEntitiesRelationshipType(relation);
        var rel = new Relationship(editId, targetId, relationshipType, true);
        _personRelationshipService.AddRelationship(rel, relation);
    }
}

