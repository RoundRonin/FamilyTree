namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public interface IStateNotifier
{
    public event Action? StateChanged;

    public void NotifyStateChanged();
}

