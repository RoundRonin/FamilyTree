using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.CommonAncestorsState.Interfaces;
using FamilyTreeBlazor.presentation.Components.Card;

namespace FamilyTreeBlazor.presentation.State.CommonAncestorsState.Substates;

public class ChooseFirstAncestorState(CommonAncestorsToolState context) : IToolState
{
    private readonly CommonAncestorsToolState _context = context;

    public void EnterState()
    {
        Console.WriteLine("Entering ChooseFirstAncestorState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting ChooseFirstAncestorState.");
    }

    public void HandleId(int id)
    {
        if (_context.CommonAncestorsCheckCandidatesIds.Count == 0)
        {
            _context.CommonAncestorsCheckCandidatesIds.Enqueue(id);
            _context.ChangeState(CommonAncestorsStateEnum.ChooseSecond);
        }
    }

    public void Cancel()
    {
        // Handle cancel action
        throw new NotImplementedException();
    }

    public RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(BlankPanel));
        builder.AddAttribute(1, "Header", "Common ancestors mode");
        builder.AddAttribute(2, "Text", "Choose the first person");
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
