
using FamilyTreeBlazor.presentation.Services.Commands;

namespace FamilyTreeBlazor.presentation.State.Interfaces;

public enum Trigger  
{
    Init,
    Cancel,
    HandleId,
    CreatePerson,
    SetRelationType
}

public enum ToolState
{
    ViewTool,
    EditTool,
    AncestorAgeTool,
    CommonAncestorsTool
}
public enum CardState
{
    Default,
    HighlightedChosen,
    Choose,
    HighlightedCommonAncestor,
}

public interface IAppStateContext 
{
    IAppState CurrentToolState { get; }
    public void RequestChangeToolState(ToolState toolState);
    public void Fire(ICommand command);
    bool DraggingOn { get; set; }
    event Action? OnChange;
}
