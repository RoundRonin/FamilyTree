using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Controllers.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.Services.Processors;
using FamilyTreeBlazor.presentation.Services.Commands;

namespace FamilyTreeBlazor.presentation.Controllers;

public class PersonController : IPersonController
{
    private readonly IPresentationService _presentationService;
    private readonly IPersonRelationshipService _personRelationshipService;
    private readonly IAncestorService _ancestorService;
    private readonly IAppStateContext _appStateContext; 

    public PersonController(
        IPresentationService presentationService,
        IPersonRelationshipService personRelationshipService,
        IAncestorService ancestorService,
        IAppStateContext appStateContext  
        )
    {
        _presentationService = presentationService;
        _personRelationshipService = personRelationshipService;
        _ancestorService = ancestorService;
        _appStateContext = appStateContext; 
    }

    public void CreateRelation(int targetId)
    {
        var processor = new CreateRelationProcessor(_personRelationshipService);
        var command = new CreateRelationCommand(targetId, processor);
        _appStateContext.CurrentToolState.Fire(command);
    }

    public void AddPerson(string name, DateTime dateTime, bool sex)
    {
        Person person = new(0, name, dateTime, sex);

        var processor = new AddPersonProcessor(_presentationService, _personRelationshipService);
        var command = new AddPersonCommand(person, processor);
        _appStateContext.CurrentToolState.Fire(command);

    }

    public int? GetAncestorAge(int AncestorId, int DescendantId)
    {
        return _ancestorService.GetAncestorAge(AncestorId, DescendantId);
    }
    public Person GetPerson(int id)
    {
        return _personRelationshipService.GetPerson(id);
    }
}