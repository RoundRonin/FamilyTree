using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class StateNotifier : IStateNotifier
{
    public event Action? StateChanged;

    public void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
