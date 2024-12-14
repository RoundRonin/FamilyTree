using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.CommonAncestorsState.Interfaces;
using FamilyTreeBlazor.presentation.Components.Card;

namespace FamilyTreeBlazor.presentation.State.CommonAncestorsState.Substates;

public class ChooseSecondAncestorState(CommonAncestorsToolState context, IAncestorService ancestorService) : IToolState
{
    private readonly CommonAncestorsToolState _context = context;
    private readonly IAncestorService _ancestorService = ancestorService;

    public void EnterState()
    {
        Console.WriteLine("Entering ChooseSecondAncestorState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting ChooseSecondAncestorState.");
    }

    public void HandleId(int id)
    {
        if (_context.CommonAncestorsCheckCandidatesIds.Count == 1 && !_context.CommonAncestorsCheckCandidatesIds.Contains(id))
        {
            _context.CommonAncestorsCheckCandidatesIds.Enqueue(id);

            int id1 = _context.CommonAncestorsCheckCandidatesIds.ElementAt(0);
            int id2 = _context.CommonAncestorsCheckCandidatesIds.ElementAt(1);

            _context.CommonAncestorsStates = _ancestorService.GetCommonAncestors(id1, id2);

            _context.ChangeState(CommonAncestorsStateEnum.View);
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
        builder.AddAttribute(2, "Text", "Choose the second person");
        builder.CloseComponent();
    };

    public RenderFragment RenderCard(Person person) => builder =>
    {
        CardState state = CardState.Default;
        if (_context.CommonAncestorsCheckCandidatesIds.Contains(person.Id))
            state = CardState.HighlightedChosen;

        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", state);
        builder.CloseComponent();
    };
}
