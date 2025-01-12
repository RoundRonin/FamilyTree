using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.EditState.Interfaces;
using FamilyTreeBlazor.presentation.Components.Card;

namespace FamilyTreeBlazor.presentation.State.EditState.Substates;

public class CreateInitialPersonState(EditToolState context) : IToolState
{
    private readonly EditToolState _context = context;

    public void EnterState()
    {
        Console.WriteLine("Entering CreateInitialPersonState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting CreateInitialPersonState.");
    }

    public void HandleId(int id)
    {
        // Handle the creation of the first person in the graph
        _context.AddPerson(id);
        _context.ChangeState(EditStateEnum.Initial);
    }

    public void Cancel()
    {
        _context.ChangeState(EditStateEnum.Initial);
    }

    public RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(EditMode));
        builder.AddAttribute(1, "Text", "first person");
        builder.CloseComponent();
    };

    public RenderFragment RenderCard(Person person) => builder =>
    {
        builder.OpenComponent(0, typeof(PersonEditCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", CardState.Default);
        builder.CloseComponent();
    };
}