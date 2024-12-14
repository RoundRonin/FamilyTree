using FamilyTreeBlazor.presentation.Infrastructure;

namespace FamilyTreeBlazor.presentation.Services;

public class AppStateService : IAppStateService
{
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




    public int? ViewId { get; set; }
    public int? EditId { get; set; }
    public bool EditCreateNew { get; set; } = false;

    private Queue<int> _ancestorAgeCandidatesIds = [];
    public Queue<int> AncestorAgeCandidatesIds { 
        get => _ancestorAgeCandidatesIds;
        set => throw new NotImplementedException(); // Maintain 2 element structure with FIFO 
    }

    private Queue<int> _commonAncestorsCheckCandidatesIds = [];
    public Queue<int> CommonAncestorsCheckCandidatesIds { 
        get => _commonAncestorsCheckCandidatesIds;
        set => throw new NotImplementedException(); // Maintain 2 element structure with FIFO
    }


    private void NotifyStateChanged()
    {
        Console.WriteLine(SelectedTool);
        OnChange?.Invoke();
    }
}
