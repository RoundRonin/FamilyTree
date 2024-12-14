using FamilyTreeBlazor.presentation.State.Interfaces;

namespace FamilyTreeBlazor.presentation.State;

public class StateNotifier : IStateNotifier
{
    public event Action? StateChanged;

    public void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
