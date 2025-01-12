using FamilyTreeBlazor.presentation.State.Interfaces;

namespace FamilyTreeBlazor.presentation.State.ViewState.Interfaces;

public interface IViewToolState : IAppState 
{
    //Nothing really
}

public enum ViewStateEnum
{
    Initial,
    View
}