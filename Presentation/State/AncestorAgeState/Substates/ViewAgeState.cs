using Microsoft.AspNetCore.Components;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;

namespace FamilyTreeBlazor.presentation.State.AncestorAgeState.Substates;

public class ViewAgeState(AncestorAgeToolState context, IPersonRelationshipService personRelationshipService) : IToolState
{
    private readonly AncestorAgeToolState _context = context;
    private readonly IPersonRelationshipService _personRelationshipService = personRelationshipService;

    public void EnterState()
    {
        Console.WriteLine("Entering ViewState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting ViewState.");
    }

    public void HandleId(int id)
    {
        if (_context.AncestorAgeCandidatesIds.Contains(id)) return;
        _context.AncestorAgeCandidatesIds.Enqueue(id);
        _context.AncestorAgeCandidatesIds.Dequeue();
        _context.NotifyStateChanged();
    }

    public void Cancel()
    {
        // Handle cancel action
        throw new NotImplementedException();
    }

    public RenderFragment RenderPanel() => builder =>
    {
        var person = _personRelationshipService.GetPerson(_context.AncestorAgeCandidatesIds.ElementAt(0));
        var ancestor = _personRelationshipService.GetPerson(_context.AncestorAgeCandidatesIds.ElementAt(1));

        builder.OpenComponent(0, typeof(AncestorAge));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "Ancestor", ancestor);
        builder.CloseComponent();
    };

    public RenderFragment RenderCard(Person person) => builder =>
    {
        CardState state = CardState.Default;
        int size = _context.AncestorAgeCandidatesIds.Count;
        if (size >= 1 && person.Id == _context.AncestorAgeCandidatesIds.ElementAt(0)) state = CardState.HighlightedChosen;
        else if (size >= 2 && person.Id == _context.AncestorAgeCandidatesIds.ElementAt(1)) state = CardState.HighlightedCommonAncestor;

        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", state);
        builder.CloseComponent();
    };
}
