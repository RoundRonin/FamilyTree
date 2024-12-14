using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class EditToolState(IStateNotifier stateNotifier) : ToolStateBase(stateNotifier), IEditToolState
{
    private int? _editId;

    public EditState _state = EditState.Initial;
    public EditState State { 
        get => _state; 
        set
        {
            _state = value;
            NotifyStateChanged();
        } 
    }

    public override void HandleId(int Id)
    {
        _editId = Id;
        NotifyStateChanged();
    }

    public override RenderFragment RenderPanel() => builder =>
    {
        switch (_state)
        {
            case EditState.Initial:
                builder.OpenComponent(0, typeof(BlankPanel));
                builder.AddAttribute(1, "Header", "Edit mode");
                builder.AddAttribute(2, "Text", "Choose a person");
                builder.CloseComponent();
                break;
            case EditState.ChoosePerson:
                builder.OpenComponent(0, typeof(EditModeDialog));
                builder.CloseComponent();
                break;
            case EditState.CreatePerson:
                builder.OpenComponent(0, typeof(EditMode));
                builder.CloseComponent();
                break;
            default:
                throw new NotImplementedException();
        }
    };

    public override RenderFragment RenderCard(Person person) => builder =>
    {
        builder.OpenComponent(0, typeof(PersonEditCard));
        builder.AddAttribute(1, "Name", person.Name);
        builder.CloseComponent();
    };

    public override object GetSpecificState() => this;
}

