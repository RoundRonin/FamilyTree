namespace FamilyTreeBlazor.presentation.State.Interfaces;

public interface IStateNotifier
{
    event Action StateChanged;
    void NotifyStateChanged();
}
