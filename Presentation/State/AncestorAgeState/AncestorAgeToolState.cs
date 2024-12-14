using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.AncestorAgeState.Substates;
using FamilyTreeBlazor.presentation.State.AncestorAgeState.Interfaces;
using FamilyTreeBlazor.presentation.Services.Commands;

namespace FamilyTreeBlazor.presentation.State.AncestorAgeState;

public class AncestorAgeToolState : IAncestorAgeToolState, IAppState 
{
    private readonly IStateNotifier _stateNotifier;
    private readonly IPersonRelationshipService _personRelationshipService;
    private readonly Dictionary<AncestorAgeStateEnum, IToolState> _states;
    private IToolState _currentState;

    public Queue<int> AncestorAgeCandidatesIds { get; } = new();

    public AncestorAgeToolState(IStateNotifier stateNotifier, IPersonRelationshipService personRelationshipService)
    {
        _stateNotifier = stateNotifier;
        _personRelationshipService = personRelationshipService;
        _states = new Dictionary<AncestorAgeStateEnum, IToolState>
        {
            { AncestorAgeStateEnum.ChooseFirst, new ChooseFirstAgeState(this) },
            { AncestorAgeStateEnum.ChooseSecond, new ChooseSecondAgeState(this) },
            { AncestorAgeStateEnum.View, new ViewAgeState(this, personRelationshipService) }
        };

        _currentState = _states[AncestorAgeStateEnum.ChooseFirst];
        _currentState.EnterState();
    }

    public void EnterState()
    {
        Console.WriteLine("Entering AncestorAgeToolState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting AncestorAgeToolState.");
    }

    public void Fire(ICommand command)
    {
        bool isExecutable = false;
        if (command is IGeneralCommandMarker) isExecutable = true;
        if (command is IAncestorAgeMarker) isExecutable = true;

        if (isExecutable) command.Execute(this, _currentState);
        else Console.WriteLine($"Command {command.GetType().Name} cannot be executed in the current state {_currentState.GetType().Name}");
    }

    public void ChangeState(AncestorAgeStateEnum newState)
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
