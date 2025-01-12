using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.CommonAncestorsState.Interfaces;
using FamilyTreeBlazor.presentation.State.CommonAncestorsState.Substates;
using FamilyTreeBlazor.presentation.Services.Commands;

namespace FamilyTreeBlazor.presentation.State.CommonAncestorsState;

public class CommonAncestorsToolState : ICommonAncestorsToolState, IAppState 
{
    private readonly IStateNotifier _stateNotifier;
    private readonly IAncestorService _ancestorService;
    private readonly IPersonRelationshipService _personRelationshipService;
    private readonly Dictionary<CommonAncestorsStateEnum, IToolState> _states;
    private IToolState _currentState;
    internal Dictionary<int, CardState>? CommonAncestorsStates { get; set; }

    public Queue<int> CommonAncestorsCheckCandidatesIds { get; } = new();

    public CommonAncestorsToolState(IStateNotifier stateNotifier, IAncestorService ancestorService, IPersonRelationshipService personRelationshipService)
    {
        _stateNotifier = stateNotifier;
        _ancestorService = ancestorService;
        _personRelationshipService = personRelationshipService;
        _states = new Dictionary<CommonAncestorsStateEnum, IToolState>
        {
            { CommonAncestorsStateEnum.ChooseFirst, new ChooseFirstAncestorState(this) },
            { CommonAncestorsStateEnum.ChooseSecond, new ChooseSecondAncestorState(this, ancestorService) },
            { CommonAncestorsStateEnum.View, new ViewCommonAncestorsState(this, ancestorService, personRelationshipService) }
        };

        _currentState = _states[CommonAncestorsStateEnum.ChooseFirst];
        _currentState.EnterState();
    }

    public void EnterState()
    {
        Console.WriteLine("Entering CommonAncestorsToolState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting CommonAncestorsToolState.");
    }

    public void Fire(ICommand command)
    {
        bool isExecutable = false;
        if (command is IGeneralCommandMarker) isExecutable = true;
        if (command is ICommonAncestorsMarker) isExecutable = true;

        if (isExecutable) command.Execute(this, _currentState);
        else Console.WriteLine($"Command {command.GetType().Name} cannot be executed in the current state {_currentState.GetType().Name}");
    }

    public void ChangeState(CommonAncestorsStateEnum newState)
    {
        if (_states.TryGetValue(newState, out IToolState? value))
        {
            _currentState.ExitState();
            _currentState = value;
            _currentState.EnterState();
            NotifyStateChanged();
        }
    }

    public RenderFragment RenderPanel() => _currentState.RenderPanel();

    public RenderFragment RenderCard(Person person) => _currentState.RenderCard(person);

    public void NotifyStateChanged()
    {
        _stateNotifier.NotifyStateChanged();
    }
}
