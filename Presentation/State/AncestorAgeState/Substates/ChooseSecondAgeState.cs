using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.AncestorAgeState.Interfaces;
using FamilyTreeBlazor.presentation.Components.Card;

namespace FamilyTreeBlazor.presentation.State.AncestorAgeState.Substates;

public class ChooseSecondAgeState(AncestorAgeToolState context) : IToolState
{
    private readonly AncestorAgeToolState _context = context;

    public void EnterState()
    {
        Console.WriteLine("Entering ChooseSecondAgeState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting ChooseSecondAgeState.");
    }

    public void HandleId(int id)
    {
        if (_context.AncestorAgeCandidatesIds.Contains(id)) return;
        _context.AncestorAgeCandidatesIds.Enqueue(id);
        _context.ChangeState(AncestorAgeStateEnum.View);
    }

    public void Cancel()
    {
        // Handle cancel action
        throw new NotImplementedException();
    }

    public RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(BlankPanel));
        builder.AddAttribute(1, "Header", "Ancestor age mode");
        builder.AddAttribute(2, "Text", "Choose an ancestor");
        builder.CloseComponent();
    };

    public RenderFragment RenderCard(Person person) => builder =>
    {
        CardState state = CardState.Default;
        if (_context.AncestorAgeCandidatesIds.Contains(person.Id)) state = CardState.HighlightedChosen;

        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", state);
        builder.CloseComponent();
    };
}