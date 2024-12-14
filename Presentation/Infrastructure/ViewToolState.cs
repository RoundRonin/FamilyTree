using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class ViewToolState(IStateNotifier stateNotifier, ITreeService treeService) : ToolStateBase(stateNotifier), IViewToolState
{
    private int? _viewId;
    private IEnumerable<Person>? _kids; 
    private IEnumerable<Person>?  _parents; 
    private Person? _spouse; 

    public ViewState _state = ViewState.Initial;
    public ViewState State { 
        get => _state; 
        set
        {
            _state = value;
            NotifyStateChanged();
        } 
    }

    public override void HandleId(int Id)
    {
        _viewId = Id;
        _kids = treeService.GetChildren(Id);
        _parents = treeService.GetParents(Id);
        _spouse = treeService.GetSpouse(Id);

        _state = ViewState.View;

        NotifyStateChanged();
    }

    public override RenderFragment RenderPanel() => builder =>
    {
        switch (_state)
        {
            case ViewState.Initial:
                builder.OpenComponent(0, typeof(BlankPanel));
                builder.AddAttribute(1, "Header", "View mode");
                builder.AddAttribute(2, "Text", "Choose a person");
                builder.CloseComponent();
                break;
            case ViewState.View:
                builder.OpenComponent(0, typeof(ViewMode));
                builder.AddAttribute(1, "PersonId", _viewId);
                builder.AddAttribute(2, "Spouse", _spouse);
                builder.AddAttribute(3, "ListKids", _kids);
                builder.AddAttribute(4, "ListParents", _parents);
                builder.CloseComponent();
                break;
            default:
                throw new NotImplementedException();
        }
    };

    public override RenderFragment RenderCard(Person person) => builder =>
    {
        CardState state = CardState.Default;
        if (person.Id == _viewId)
        {
            state = CardState.HighlightedChosen;
        }

        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", state);
        builder.CloseComponent();
    };

    public override object GetSpecificState() => this;
}

