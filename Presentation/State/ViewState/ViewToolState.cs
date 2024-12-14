using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.ViewState.Interfaces;
using FamilyTreeBlazor.presentation.State.ViewState.Substates;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;
using FamilyTreeBlazor.presentation.Services.Commands;

namespace FamilyTreeBlazor.presentation.State.ViewState;

public class ViewToolState : IViewToolState, IAppState 
{
    private readonly IStateNotifier _stateNotifier;
    private readonly IRelationshipInfoService _relationshipInfoService;
    private readonly Dictionary<ViewStateEnum, IToolState> _states;
    private IToolState _currentState;
    internal int? ViewId { get; set; }
    internal IEnumerable<Person>? Kids { get; set; }
    internal IEnumerable<Person>? Parents { get; set; }
    internal Person? Spouse { get; set; }

    public ViewToolState(IStateNotifier stateNotifier, IRelationshipInfoService relationshipInfoService)
    {
        _stateNotifier = stateNotifier;
        _relationshipInfoService = relationshipInfoService;
        _states = new Dictionary<ViewStateEnum, IToolState>
        {
            { ViewStateEnum.Initial, new InitialViewState(this, relationshipInfoService) },
            { ViewStateEnum.View, new ViewInfoState(this, relationshipInfoService) }
        };

        _currentState = _states[ViewStateEnum.Initial];
        _currentState.EnterState();
    }

    public void EnterState()
    {
        Console.WriteLine("Entering ViewToolState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting ViewToolState.");
    }

    public void Fire(ICommand command)
    {
        bool isExecutable = false;
        if (command is IGeneralCommandMarker) isExecutable = true;
        if (command is IViewMarker) isExecutable = true;

        if (isExecutable) command.Execute(this, _currentState);
        else Console.WriteLine($"Command {command.GetType().Name} cannot be executed in the current state {_currentState.GetType().Name}");
    }

    public void ChangeState(ViewStateEnum newState)
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
