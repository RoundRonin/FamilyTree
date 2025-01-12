using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.State.ViewState.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.State.ViewState.Substates;

public class InitialViewState(ViewToolState context, IRelationshipInfoService relationshipInfoService) : IToolState
{
    private readonly ViewToolState _context = context;
    private readonly IRelationshipInfoService _relationshipInfoService = relationshipInfoService;

    public void EnterState()
    {
        Console.WriteLine("Entering InitialViewState.");
    }

    public void ExitState()
    {
        Console.WriteLine("Exiting InitialViewState.");
    }

    public void HandleId(int id)
    {
        // Populate the specific person's details
        _context.ViewId = id;
        _context.Kids = _relationshipInfoService.GetChildren(id);
        _context.Parents = _relationshipInfoService.GetParents(id);
        _context.Spouse = _relationshipInfoService.GetSpouse(id);

        // Change state to ViewSpecificInfoState
        _context.ChangeState(ViewStateEnum.View);
    }

    public void Cancel()
    {
        // Handle cancel action
        throw new NotImplementedException();
    }

    public RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(BlankPanel));
        builder.AddAttribute(1, "Header", "View mode");
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
