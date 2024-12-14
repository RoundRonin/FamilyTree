using FamilyTreeBlazor.BLL.DTOs;

namespace FamilyTreeBlazor.presentation.Services;

public enum Tool
{
    View,
    Edit,
    AncestorAge,
    CommonAncestors,
    DeleteTree
}

public class AppState
{
    public TreeCacheDTO CachedTree { get; private set; } = new TreeCacheDTO();
    public Tool SelectedTool { get; set; } = Tool.View;
    public event Action? OnChange;

    public void InitializeCachedTree(TreeCacheDTO tree)
    {
        CachedTree = tree;
        NotifyStateChanged();
    }

    public void UpdateCachedTree(TreeCacheDTO tree)
    {
        CachedTree = tree;
        NotifyStateChanged();
    }

    public void NotifyStateChanged()
    {
        Console.WriteLine(SelectedTool);
        OnChange?.Invoke();
    }
}
