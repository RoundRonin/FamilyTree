using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class EditToolState(IStateNotifier stateNotifier, ITreeService treeService) : ToolStateBase(stateNotifier), IEditToolState
{
    private int _editId;

    private Dictionary<int, bool> _relations = [];

    private EditState _state = EditState.Initial;
    private Relation _relationState = Relation.Parent;

    public int EditId { get => _editId; }
    public EditState State { 
        get => _state; 
        set
        {
            _state = value;
            NotifyStateChanged();
        } 
    }
    public Relation RelationState { 
        get => _relationState; 
        set
        {
            _relationState = value;
            NotifyStateChanged();
        } 
    }

    public void SetState(EditState state, Relation relation)
    {
        _state = state;
        _relationState = relation;
        NotifyStateChanged();
    }

    public override void HandleId(int Id)
    {
        _editId = Id;
        //PersonListDTO _parents = treeService.GetParents(Id);
        //PersonListDTO _kids = treeService.GetKids(Id);
        //Person? _spouse = treeService.GetSpouse(Id);

        //_relations.Clear();

        //var parentsDictionary = _parents.Persons
        //    .Where(p => p.Id.HasValue) // Filter out null Ids
        //    .ToDictionary(p => p.Id.Value, p => true);

        //var kidsDictionary = _kids.Persons
        //    .Where(p => p.Id.HasValue) // Filter out null Ids
        //    .ToDictionary(p => p.Id.Value, p => true);

        //foreach (var kvp in parentsDictionary)
        //{
        //    _relations[kvp.Key] = kvp.Value; 
        //}

        //foreach (var kvp in kidsDictionary)
        //{
        //    _relations[kvp.Key] = kvp.Value;
        //}

        //if (_spouse != null)
        //{
        //    _relations[_spouse.Id] = true; 
        //}

        NotifyStateChanged();
    }

    public override RenderFragment RenderPanel() => builder =>
    {
        string text = "default";
        text = _relationState switch
        {
            Relation.Child => "child",
            Relation.Spouse => "spouse",
            Relation.Parent => "parent",
            _ => throw new NotImplementedException(),
        };

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
                builder.AddAttribute(1, "Text", text);
                builder.CloseComponent();
                break;
            case EditState.CreatePerson:
                builder.OpenComponent(0, typeof(EditMode));
                builder.AddAttribute(1, "Text", text);
                builder.CloseComponent();
                break;
            default:
                throw new NotImplementedException();
        }
    };

    public override RenderFragment RenderCard(Person person) => builder =>
    {
        CardState state = CardState.Default;
        if (person.Id == _editId)
        {
            state = CardState.HighlightedChosen;
        }

        // Define which buttons are usable
        DisabledRelations disabledRelations = new();
        disabledRelations.Enable(); // TEMP


        //PersonListDTO parents = treeService.GetParents(person.Id);
        //PersonListDTO kids = treeService.GetKids(person.Id);
        //Person? spouse = treeService.GetSpouse(person.Id);

        //if (parents.Persons.Count < 2) disabledRelations.Parent = true;
        //if (spouse != null) disabledRelations.Spouse = true;
        //// Child check is not needed. We can have a million kids

        switch (_state)
        {
            case EditState.Initial:
                builder.OpenComponent(0, typeof(PersonEditCard));
                builder.AddAttribute(1, "Name", person.Name);
                builder.AddAttribute(2, "State", CardState.Default);
                builder.AddAttribute(3, "DisabledButtons", disabledRelations);
                break;
            case EditState.ChoosePerson:
                if (person.Id != _editId)
                {
                    // Check if a person is already a relative
                    if (_relations.TryGetValue(person.Id, out var relation))
                    {
                        disabledRelations.Disable();
                    }

                    // Todo check if a person is an ancestor of any kind for the _editId


                    builder.OpenComponent(0, typeof(PersonAddCard));
                    builder.AddAttribute(1, "Person", person);
                    builder.AddAttribute(2, "State", state);
                    builder.AddAttribute(3, "UnavailableRelations", disabledRelations);
                } 
                else
                {
                    builder.OpenComponent(0, typeof(PersonEditCard));
                    builder.AddAttribute(1, "Name", person.Name);
                    builder.AddAttribute(2, "State", state);
                    builder.AddAttribute(3, "DisabledButtons", disabledRelations);
                }
                break;
            case EditState.CreatePerson:
                builder.OpenComponent(0, typeof(PersonEditCard));
                builder.AddAttribute(1, "Name", person.Name);
                builder.AddAttribute(2, "State", state);
                builder.AddAttribute(3, "DisabledButtons", disabledRelations);
                break;
            default:
                break;
        }

        builder.CloseComponent();
    };

    public override object GetSpecificState() => this;
}

