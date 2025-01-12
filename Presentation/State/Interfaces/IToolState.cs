namespace FamilyTreeBlazor.presentation.State.Interfaces;

public interface IToolState : IState
{
    void HandleId(int id);
    void Cancel();
}
