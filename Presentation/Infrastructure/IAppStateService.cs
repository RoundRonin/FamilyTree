namespace FamilyTreeBlazor.presentation.Infrastructure;

public enum Tool
{
    View,
    Edit,
    AncestorAge,
    CommonAncestors
}

public interface IAppStateService
{
    public Tool SelectedTool {  get; set; } 

    public bool DraggingOn {  get; set; }

    public event Action? OnChange;
}
