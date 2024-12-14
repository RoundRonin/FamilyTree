using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class ViewToolState(IStateNotifier stateNotifier) : ToolStateBase(stateNotifier), IViewToolState
{
    private int? _viewId;
    public int? ViewId
    {
        get => _viewId;
        set
        {
            _viewId = value;
            NotifyStateChanged();
        }
    }

    public override RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(ViewMode));
        builder.CloseComponent();
    };

    public override RenderFragment RenderCard(string name, DateTime birthDay) => builder =>
    {
        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Name", name);
        builder.AddAttribute(2, "BirthDay", birthDay);
        builder.CloseComponent();
    };

    public override object GetSpecificState() => this;
}

