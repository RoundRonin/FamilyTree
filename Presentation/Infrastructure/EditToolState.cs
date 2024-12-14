using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class EditToolState(IStateNotifier stateNotifier) : ToolStateBase(stateNotifier), IEditToolState
{
    private int? _editId;
    public int? EditId
    {
        get => _editId;
        set
        {
            _editId = value;
            NotifyStateChanged();
        }
    }

    private bool _editCreateNew = false;
    public bool EditCreateNew
    {
        get => _editCreateNew;
        set
        {
            _editCreateNew = value;
            NotifyStateChanged();
        }
    }

    public override RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(EditMode));
        builder.CloseComponent();
    };

    public override RenderFragment RenderCard(string name, DateTime birthDay) => builder =>
    {
        builder.OpenComponent(0, typeof(PersonEditCard));
        builder.AddAttribute(1, "Name", name);
        builder.CloseComponent();
    };

    public override object GetSpecificState() => this;
}

