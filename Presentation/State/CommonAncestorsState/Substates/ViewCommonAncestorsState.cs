using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.Components.Card;

namespace FamilyTreeBlazor.presentation.State.CommonAncestorsState.Substates;

public class ViewCommonAncestorsState(CommonAncestorsToolState context, IAncestorService ancestorService, IPersonRelationshipService personRelationshipService) : IToolState
{
    private readonly CommonAncestorsToolState _context = context;
    private readonly IAncestorService _ancestorService = ancestorService;
    private readonly IPersonRelationshipService _personRelationshipService = personRelationshipService;

    public void EnterState()
    {
        Console.WriteLine("Entering ViewCommonAncestorsState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting ViewCommonAncestorsState.");
    }

    public void HandleId(int id)
    {
        if (!_context.CommonAncestorsCheckCandidatesIds.Contains(id))
        {
            _context.CommonAncestorsCheckCandidatesIds.Enqueue(id);
            _context.CommonAncestorsCheckCandidatesIds.Dequeue();

            int id1 = _context.CommonAncestorsCheckCandidatesIds.ElementAt(0);
            int id2 = _context.CommonAncestorsCheckCandidatesIds.ElementAt(1);

            _context.CommonAncestorsStates = _ancestorService.GetCommonAncestors(id1, id2);

            _context.NotifyStateChanged();
        }
    }

    public void Cancel()
    {
        // Handle cancel action
        throw new NotImplementedException();
    }

    public RenderFragment RenderPanel() => builder =>
    {
        var personA = _personRelationshipService.GetPerson(_context.CommonAncestorsCheckCandidatesIds.ElementAt(0));
        var personB = _personRelationshipService.GetPerson(_context.CommonAncestorsCheckCandidatesIds.ElementAt(1));

        builder.OpenComponent(0, typeof(CommonAncestors));
        builder.AddAttribute(1, "PersonA", personA);
        builder.AddAttribute(2, "PersonB", personB);
        builder.AddAttribute(3, "Ancestors", _context.CommonAncestorsStates.Keys.Select(id => _personRelationshipService.GetPerson(id)));
        builder.CloseComponent();
    };

    public RenderFragment RenderCard(Person person) => builder =>
    {
        CardState state = CardState.Default;
        if (_context.CommonAncestorsCheckCandidatesIds.Contains(person.Id))
        {
            state = CardState.HighlightedChosen;
        }

        if (_context.CommonAncestorsStates != null && _context.CommonAncestorsStates.TryGetValue(person.Id, out var ancestorState))
        {
            state = ancestorState;
        }

        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", state);
        builder.CloseComponent();
    };
}
