using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

namespace FamilyTreeBlazor.presentation.Services.Interfaces;

public enum CardState
{
    Default,
    HighlightedChosen,
    Choose,
    HighlightedCommonAncestor,
}

public interface IAppStateService
{
    IToolState CurrentToolState { get; set; }
    
    event Action? OnChange;

    void ChangeToolState<T>() where T : IToolState;
    T GetSpecificState<T>() where T : class, IToolState;

    public bool DraggingOn {  get; set; }
}
