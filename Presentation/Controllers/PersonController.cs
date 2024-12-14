using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Controllers.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.Infrastructure;
using FamilyTreeBlazor.presentation.Infrastructure.Helpers;

namespace FamilyTreeBlazor.presentation.Controllers;

public class PersonController : IPersonController
{
    private IPresentationService _presentationService;
    private IPersonRelationshipService _personRelationshipService;
    private IAncestorService _ancestorService;
    private IAppStateService _appState; // TODO remove

    public PersonController(
        IPresentationService presentationService,
        IPersonRelationshipService personRelationshipService,
        IAncestorService ancestorService,
        IAppStateService appState  // TODO remove
        )
    {
        _presentationService = presentationService;
        _personRelationshipService = personRelationshipService;
        _ancestorService = ancestorService;
        _appState = appState; // TODO remove
    }

    public void CreateRelation(int targetId)
    {
        Relation relType = _appState.GetSpecificState<EditToolState>().RelationState;
        Models.RelationshipType relationshipType;
        relationshipType = RelationTranslator.ToEntitiesRelationshipType(relType);

        Relationship rel = new(_appState.GetSpecificState<EditToolState>().EditId, targetId, relationshipType, true);

        _personRelationshipService.AddRelationship(rel, relType);

        _appState.GetSpecificState<EditToolState>().State = EditState.Initial;
    }

    public void AddPerson(string name, DateTime dateTime, bool sex)
    {
        if (_appState.GetSpecificState<EditToolState>().State == EditState.CreateInitialPerson)
        {
            Person person = new(0, name, dateTime, sex);
            _presentationService.AddInitialPerson(person);
            _appState.GetSpecificState<EditToolState>().State = EditState.Initial;
            _appState.ChangeToolState<ViewToolState>();
        }
        else
        {
            Relation relType = _appState.GetSpecificState<EditToolState>().RelationState;
            Models.RelationshipType relationshipType;

            relationshipType = RelationTranslator.ToEntitiesRelationshipType(relType);

            Person person = new(0, name, dateTime, sex);
            Relationship rel = new(_appState.GetSpecificState<EditToolState>().EditId, person.Id, relationshipType, true);

            _personRelationshipService.AddPersonRelationship(person, rel, relType);
        }
    }

    public int? GetAncestorAge(int Id1, int Id2)
    {
        return _ancestorService.GetAncestorAge(Id1, Id2);
    }
    public Person GetPerson(int id)
    {
        return _personRelationshipService.GetPerson(id);
    }
}