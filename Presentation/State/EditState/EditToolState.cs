using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;
using FamilyTreeBlazor.presentation.State.EditState.Substates;
using FamilyTreeBlazor.presentation.Services.Commands;

namespace FamilyTreeBlazor.presentation.State.EditState;

public class EditToolState : IEditToolState, IAppState 
{
    private readonly IStateNotifier _stateNotifier;
    private readonly IAncestorService _ancestorService;
    private readonly IPersonRelationshipService _personRelationshipService;
    private readonly IRelationshipInfoService _relationshipInfoService;
    private readonly Dictionary<EditStateEnum, IToolState> _states;
    private IToolState _currentState;
    private int _editId;

    public int EditId { get => _editId; } 

    internal Dictionary<int, bool> Relations { get; set; } = [];
    internal Dictionary<int, CardState> Ancestors { get; set; } = [];
    internal Relation CurrentRelation { get; set; }
    internal Person EditPerson { get; set; }

    public EditToolState(IStateNotifier stateNotifier, IAncestorService ancestorService, IPersonRelationshipService personRelationshipService, IRelationshipInfoService relationshipInfoService)
    {
        _stateNotifier = stateNotifier;
        _ancestorService = ancestorService;
        _personRelationshipService = personRelationshipService;
        _relationshipInfoService = relationshipInfoService;
        _states = new Dictionary<EditStateEnum, IToolState>
        {
            { EditStateEnum.Initial, new InitialEditState(this, relationshipInfoService) },
            { EditStateEnum.ChoosePerson, new ChoosePersonState (this, relationshipInfoService) },
            { EditStateEnum.CreatePerson, new CreatePersonState (this, relationshipInfoService) },
            { EditStateEnum.CreateInitialPerson, new CreateInitialPersonState (this) }
        };

        _currentState = _states[EditStateEnum.Initial];
        _currentState.EnterState();
    }

    public void EnterState()
    {
        Console.WriteLine("Entering EditToolState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting EditToolState.");
    }

    public void Fire(ICommand command)
    {
        bool isExecutable = false;
        if (command is IGeneralCommandMarker) isExecutable = true;
        if (command is IEditMarker) isExecutable = true;

        if (isExecutable) command.Execute(this, _currentState);
        else Console.WriteLine($"Command {command.GetType().Name} cannot be executed in the current state {_currentState.GetType().Name}");
    }

    public void ChangeState(EditStateEnum newState)
    {
        if (_states.TryGetValue(newState, out IToolState? value))
        {
            _currentState.ExitState();
            _currentState = value;
            _currentState.EnterState();
            NotifyStateChanged();
        }
    }

    public void AddPerson(int id)
    {
        _editId = id;
        EditPerson = _personRelationshipService.GetPerson(id);
        var parents = _relationshipInfoService.GetParents(id);
        var kids = _relationshipInfoService.GetChildren(id);
        var spouse = _relationshipInfoService.GetSpouse(id);

        Relations.Clear();
        Ancestors.Clear();

        var ancestors = _ancestorService.GetPersonAncestors(id);
        Ancestors = ancestors.ToDictionary(k => k.Key, v => v.Value);

        foreach (var parent in parents) Relations[parent.Id] = true;
        foreach (var kid in kids) Relations[kid.Id] = true;
        if (spouse != null) Relations[spouse.Id] = true;

        NotifyStateChanged();
    }

    public RenderFragment RenderPanel() => _currentState.RenderPanel();

    public RenderFragment RenderCard(Person person) => _currentState.RenderCard(person);

    public void NotifyStateChanged()
    {
        _stateNotifier.NotifyStateChanged();
    }

    public void SetRelationState(Relation relation)
    {
        CurrentRelation = relation;
        NotifyStateChanged();
    }
}