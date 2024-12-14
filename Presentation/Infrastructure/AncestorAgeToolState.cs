using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class AncestorAgeToolState(IStateNotifier stateNotifier) : ToolStateBase(stateNotifier), IAncestorAgeToolState
{
    private readonly Queue<int> _ancestorAgeCandidatesIds = new();

    public Queue<int> AncestorAgeCandidatesIds => _ancestorAgeCandidatesIds;

    public void AddCandidateId(int id)
    {
        // Maintain a queue of two elements
        if (_ancestorAgeCandidatesIds.Count == 2)
        {
            _ancestorAgeCandidatesIds.Dequeue(); 
        }
        _ancestorAgeCandidatesIds.Enqueue(id);
        NotifyStateChanged();
    }

    public override RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(AncestorAge));
        builder.CloseComponent();
    };

    public override RenderFragment RenderCard(string name, DateTime birthDay) => builder =>
    {
        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Name", name);
        builder.AddAttribute(2, "BirthDay", birthDay);
        builder.CloseComponent();
    };

    public override object GetSpecificState() => this;
}
