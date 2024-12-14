using FamilyTreeBlazor.presentation.Infrastructure.Helpers;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;

namespace FamilyTreeBlazor.presentation.Services.Processors;

public class AddPersonProcessor(IPresentationService presentationService, IPersonRelationshipService personRelationshipService)
{
    private readonly IPresentationService _presentationService = presentationService;
    private readonly IPersonRelationshipService _personRelationshipService = personRelationshipService;

    public void AddInitialPerson(Person person)
    {
        _presentationService.AddInitialPerson(person);
    }

    public void AddPersonWithRelation(int EditId, Relation relation, Person person)
    {
        RelationshipType relationshipType = RelationTranslator.ToEntitiesRelationshipType(relation); 
        Relationship rel = new(EditId, person.Id, relationshipType, true);
        _personRelationshipService.AddPersonRelationship(person, rel, relation);
    }
}

