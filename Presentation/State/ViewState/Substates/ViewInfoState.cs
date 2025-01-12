using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.State.ViewState.Substates;

public class ViewInfoState(ViewToolState context, IRelationshipInfoService relationshipInfoService) : IToolState
{
    private readonly ViewToolState _context = context;
    private readonly IRelationshipInfoService _relationshipInfoService = relationshipInfoService;

    public void EnterState()
    {
        Console.WriteLine("Entering ViewInfoState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting ViewInfoState.");
    }

    public void HandleId(int id)
    {
        _context.ViewId = id;
        _context.Kids = _relationshipInfoService.GetChildren(id);
        _context.Parents = _relationshipInfoService.GetParents(id);
        _context.Spouse = _relationshipInfoService.GetSpouse(id);

        _context.NotifyStateChanged();
    }

    public void Cancel()
    {
        // Handle cancel action
        throw new NotImplementedException();
    }

    public RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(ViewMode));
        builder.AddAttribute(1, "PersonId", _context.ViewId);
        builder.AddAttribute(2, "Spouse", _context.Spouse);
        builder.AddAttribute(3, "ListKids", _context.Kids);
        builder.AddAttribute(4, "ListParents", _context.Parents);
        builder.CloseComponent();
    };

    public RenderFragment RenderCard(Person person) => builder =>
    {
        CardState state = person.Id == _context.ViewId ? CardState.HighlightedChosen : CardState.Default;

        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", state);
        builder.CloseComponent();
    };
}
