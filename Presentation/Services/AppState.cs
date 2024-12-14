using System.Threading;
using FamilyTreeBlazor.BLL.DTOs;
using System.Diagnostics.Eventing.Reader;

namespace FamilyTreeBlazor.presentation.Services;

public enum Tool
{
    View,
    Edit,
    AncestorAge,
    CommonAncestors
}

public class AppState
{
    public TreeCacheDTO CachedTree { get; private set; } = new TreeCacheDTO();
    
    private Tool _selectedTool = Tool.View;
    public Tool SelectedTool
    {
        get => _selectedTool;
        set
        {
            _selectedTool = value;
            NotifyStateChanged();
        }
    }

    private bool _draggingOn = false;
    public bool DraggingOn
    {
        get => _draggingOn;
        set
        {
            _draggingOn = value;
            Console.WriteLine("Changed dragging setting: " + _draggingOn);
            NotifyStateChanged();
        }
    }

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

    private void NotifyStateChanged()
    {
        Console.WriteLine(SelectedTool);
        OnChange?.Invoke();
    }
}
