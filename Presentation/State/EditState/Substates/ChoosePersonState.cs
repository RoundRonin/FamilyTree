using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;

namespace FamilyTreeBlazor.presentation.State.EditState.Substates;

public class ChoosePersonState(EditToolState context, IRelationshipInfoService relationshipInfoService) : IToolState
{
    private readonly EditToolState _context = context;
    private readonly IRelationshipInfoService _relationshipInfoService = relationshipInfoService;

    public void EnterState()
    {
        Console.WriteLine("Entering ChoosePersonState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting ChoosePersonState.");
    }

    public void HandleId(int id)
    {
        if (id != _context.EditPerson.Id)
        {
            _context.AddPerson(id);
            _context.ChangeState(EditStateEnum.CreatePerson);
        }
    }

    public void Cancel()
    {
        _context.ChangeState(EditStateEnum.Initial);
    }

    public RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(EditModeDialog));
        builder.AddAttribute(1, "Text", _context.CurrentRelation.ToString().ToLower());
        builder.CloseComponent();
    };

    public RenderFragment RenderCard(Person person) => builder =>
    {
        // Set of rules needed to render a card correctly. Including checks for existing relationships, sex and so on
        CardState state = CardState.Default;
        if (person.Id == _context.EditPerson.Id)
        {
            state = CardState.HighlightedChosen;
        }

        DisabledRelations disabledRelations = new();
        var parents = _relationshipInfoService.GetParents(person.Id);
        var kids = _relationshipInfoService.GetChildren(person.Id);
        var spouse = _relationshipInfoService.GetSpouse(person.Id);

        if (parents.Count() < 2) disabledRelations.Parent = false;
        if (spouse == null) disabledRelations.Spouse = false;
        disabledRelations.Child = false;

        if (person.Id != _context.EditId)
        {
            bool disableAddition = _context.Relations.ContainsKey(person.Id);

            if (_context.CurrentRelation == Relation.Spouse && person.Sex == _context.EditPerson.Sex)
                disableAddition = true;

            if (_context.Ancestors.ContainsKey(person.Id))
                disableAddition = true;

            if (_context.CurrentRelation == Relation.Child && person.BirthDateTime < _context.EditPerson.BirthDateTime)
                disableAddition = true;

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

        builder.CloseComponent();
    };
}
