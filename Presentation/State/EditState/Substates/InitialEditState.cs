using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;
using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Services.Interfaces;

namespace FamilyTreeBlazor.presentation.State.EditState.Substates;

public class InitialEditState(EditToolState context, IRelationshipInfoService relationshipInfoService) : IToolState
{
    private readonly EditToolState _context = context;
    private readonly IRelationshipInfoService _relationshipInfoService = relationshipInfoService;

    public void EnterState()
    {
        Console.WriteLine("Entering InitialEditState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting InitialEditState.");
    }

    public void HandleId(int id)
    {
        _context.AddPerson(id);
        _context.ChangeState(EditStateEnum.ChoosePerson);
    }

    public void Cancel()
    {
        // Handle cancel action
    }

    public RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(BlankPanel));
        builder.AddAttribute(1, "Header", "Edit mode");
        builder.AddAttribute(2, "Text", "Choose a person");
        builder.CloseComponent();
    };

    public RenderFragment RenderCard(Person person) => builder =>
    {

        CardState state = CardState.Default;
        if (person.Id == _context.EditId)
        {
            state = CardState.HighlightedChosen;
        }

        // Define which buttons are usable
        DisabledRelations disabledRelations = new();

        IEnumerable<Person> parents = _relationshipInfoService.GetParents(person.Id);
        IEnumerable<Person> kids = _relationshipInfoService.GetChildren(person.Id);
        Person? spouse = _relationshipInfoService.GetSpouse(person.Id);

        if (parents.Count() < 2) disabledRelations.Parent = false;
        if (spouse == null) disabledRelations.Spouse = false;
        disabledRelations.Child = false;
        // Child check is not needed. We can have a million kids

        builder.OpenComponent(0, typeof(PersonEditCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", state);
        builder.AddAttribute(3, "DisabledButtons", disabledRelations);
        builder.CloseComponent();
    };
}
