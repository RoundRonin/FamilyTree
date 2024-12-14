using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.AncestorAgeState.Interfaces;
using FamilyTreeBlazor.presentation.State.CommonAncestorsState.Interfaces;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;
using FamilyTreeBlazor.presentation.State.ViewState.Interfaces;
using FamilyTreeBlazor.presentation.Services.Commands;

namespace FamilyTreeBlazor.presentation.State;

public class AppStateContext : IAppStateContext
{
    private readonly IStateNotifier _stateNotifier;
    private readonly Dictionary<ToolState, IAppState> _toolStates;
    private IAppState _currentToolState;
    private bool _isChangingState = false;

    public AppStateContext(IStateNotifier stateNotifier, IViewToolState viewToolState, IEditToolState editToolState,
        IAncestorAgeToolState ancestorAgeToolState, ICommonAncestorsToolState commonAncestorsToolState)
    {
        _stateNotifier = stateNotifier;

        _toolStates = new Dictionary<ToolState, IAppState>
        {
            { ToolState.ViewTool, viewToolState },
            { ToolState.EditTool, editToolState },
            { ToolState.AncestorAgeTool, ancestorAgeToolState },
            { ToolState.CommonAncestorsTool, commonAncestorsToolState }
        };

        _currentToolState = viewToolState; // Default state
        _currentToolState.EnterState();
    }

    public IAppState CurrentToolState
    {
        get => _currentToolState;
        private set
        {
            if (_isChangingState) return;
            _isChangingState = true;

            _currentToolState = value;
            _stateNotifier.NotifyStateChanged();

            _isChangingState = false;
        }
    }

    public event Action? OnChange
    {
        add => _stateNotifier.StateChanged += value;
        remove => _stateNotifier.StateChanged -= value;
    }

    // Method to request state change
    public void RequestChangeToolState(ToolState toolState)
    {
        if (_toolStates.TryGetValue(toolState, out var toolStateInstance))
        {
            _currentToolState.ExitState();
            CurrentToolState = toolStateInstance;
            _currentToolState.EnterState();
        }
    }

    // Handle triggers
    public void Fire(ICommand command)
    {
        _currentToolState.Fire(command);
    }

    // Manage global states like dragging
    private bool _draggingOn = false;
    public bool DraggingOn
    {
        get => _draggingOn;
        set
        {
            _draggingOn = value;
            _stateNotifier.NotifyStateChanged();
        }
    }
}
