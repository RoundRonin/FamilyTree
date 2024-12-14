using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.AncestorAgeState.Interfaces;
using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;

namespace FamilyTreeBlazor.presentation.State.AncestorAgeState.Substates;

public class ChooseFirstAgeState(AncestorAgeToolState context) : IToolState
{
    private readonly AncestorAgeToolState _context = context;

    public void EnterState()
    {
        Console.WriteLine("Entering ChooseFirstAgeState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting ChooseFirstAgeState.");
    }

    public void HandleId(int id)
    {
        _context.AncestorAgeCandidatesIds.Clear();
        _context.AncestorAgeCandidatesIds.Enqueue(id);
        _context.ChangeState(AncestorAgeStateEnum.ChooseSecond);
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
        builder.AddAttribute(2, "Text", "Choose a person");
        builder.CloseComponent();
    };

    public RenderFragment RenderCard(Person person) => builder =>
    {
        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", CardState.Default);
        builder.CloseComponent();
    };
}
