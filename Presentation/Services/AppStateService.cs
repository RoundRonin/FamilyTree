using FamilyTreeBlazor.presentation.Infrastructure;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;

namespace FamilyTreeBlazor.presentation.Services;

public class AppStateService : IAppStateService
{
    private readonly IStateNotifier _stateNotifier;
    private readonly Dictionary<Type, IToolState> _toolStates;
    private IToolState _currentToolState;
    private bool _isChangingState = false;

    public AppStateService(IStateNotifier stateNotifier, IViewToolState viewToolState, IEditToolState editToolState,
        IAncestorAgeToolState ancestorAgeToolState, ICommonAncestorsToolState commonAncestorsToolState)
    {
        _stateNotifier = stateNotifier;

        _toolStates = new Dictionary<Type, IToolState>
        {
            { typeof(ViewToolState), viewToolState },
            { typeof(EditToolState), editToolState },
            { typeof(AncestorAgeToolState), ancestorAgeToolState },
            { typeof(CommonAncestorsToolState), commonAncestorsToolState }
        };

        _currentToolState = viewToolState; // Default state
    }

    public IToolState CurrentToolState
    {
        get => _currentToolState;
        set
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
        remove => _stateNotifier.StateChanged += value;
    }

    public void ChangeToolState<T>() where T : IToolState
    {
        if (_toolStates.TryGetValue(typeof(T), out var toolState))
        {
            _currentToolState = toolState;
            _stateNotifier.NotifyStateChanged();
        }
    }

    public T GetSpecificState<T>() where T : class, IToolState
    {
        return _currentToolState as T;
    }

    // Additional global states declared below
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
