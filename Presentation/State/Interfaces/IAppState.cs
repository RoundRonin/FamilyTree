using FamilyTreeBlazor.presentation.Services.Commands;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;

namespace FamilyTreeBlazor.presentation.State.Interfaces;


public interface IAppState : IState
{
    public void Fire(ICommand command);
    void NotifyStateChanged();
}