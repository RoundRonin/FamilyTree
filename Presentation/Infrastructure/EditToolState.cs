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
    private Dictionary<int, CardState> _ancestors = [];

    private Person editPerson;
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
        editPerson = treeService.GetPerson(Id);
        IEnumerable<Person> _parents = treeService.GetParents(Id);
        IEnumerable<Person> _kids = treeService.GetChildren(Id);
        Person? _spouse = treeService.GetSpouse(Id);

        _relations.Clear();
        _ancestors.Clear();

        var ancestors = treeService.GetPersonAncestors(Id);
        _ancestors = ancestors;
        foreach (var ancestor in _ancestors) Console.WriteLine(ancestor);

        var parentsDictionary = _parents
            .ToDictionary(p => p.Id, p => true);

        var kidsDictionary = _kids
            .ToDictionary(p => p.Id, p => true);

        foreach (var kvp in parentsDictionary)
        {
            _relations[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in kidsDictionary)
        {
            _relations[kvp.Key] = kvp.Value;
        }

        if (_spouse != null)
        {
            _relations[_spouse.Id] = true;
        }

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


        IEnumerable<Person> parents = treeService.GetParents(person.Id);
        IEnumerable<Person> kids = treeService.GetChildren(person.Id);
        Person? spouse = treeService.GetSpouse(person.Id);

        if (parents.Count() < 2) disabledRelations.Parent = false;
        if (spouse == null) disabledRelations.Spouse = false;
        disabledRelations.Child = false;
        // Child check is not needed. We can have a million kids

        switch (_state)
        {
            case EditState.Initial:
                builder.OpenComponent(0, typeof(PersonEditCard));
                builder.AddAttribute(1, "Person", person);
                builder.AddAttribute(2, "State", CardState.Default);
                builder.AddAttribute(3, "DisabledButtons", disabledRelations);
                break;
            case EditState.ChoosePerson:
                if (person.Id != _editId)
                {
                    bool disableAddition = false;
                    // Check if a person is already a relative
                    if (_relations.TryGetValue(person.Id, out var relation))
                    {
                        disableAddition = true;
                    }

                    // We support traditional values on this one
                    if (_relationState == Relation.Spouse 
                        && person.Sex == editPerson.Sex) 
                        disableAddition = true;

                    // Check if a person is an ancestor
                    if (_ancestors.TryGetValue(person.Id, out var ancestors))
                    {
                        disableAddition = true;
                    }

                    // For kids -- check if a person is older
                    if (_relationState == Relation.Child && person.BirthDateTime < editPerson.BirthDateTime)
                    {
                        disableAddition = true;
                    }


                    builder.OpenComponent(0, typeof(PersonAddCard));
                    builder.AddAttribute(1, "Person", person);
                    builder.AddAttribute(2, "State", state);
                    builder.AddAttribute(3, "DisableButton", disableAddition);
                } 
                else
                {
                    builder.OpenComponent(0, typeof(PersonEditCard));
                    builder.AddAttribute(1, "Person", person);
                    builder.AddAttribute(2, "State", state);
                    builder.AddAttribute(3, "DisabledButtons", disabledRelations);
                }
                break;
            case EditState.CreatePerson:
                builder.OpenComponent(0, typeof(PersonEditCard));
                builder.AddAttribute(1, "Person", person);
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

