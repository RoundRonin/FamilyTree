using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class CommonAncestorsToolState(IStateNotifier stateNotifier) : ToolStateBase(stateNotifier), ICommonAncestorsToolState
{
    private readonly Queue<int> _commonAncestorsCheckCandidatesIds = new();
    public Queue<int> CommonAncestorsCheckCandidatesIds => _commonAncestorsCheckCandidatesIds;
    public void AddCandidateId(int id)
    {
        // Maintain a queue of two elements
        if (_commonAncestorsCheckCandidatesIds.Count == 2)
        {
            _commonAncestorsCheckCandidatesIds.Dequeue(); 
        }
        _commonAncestorsCheckCandidatesIds.Enqueue(id);
        NotifyStateChanged();
    }
    public override RenderFragment RenderPanel() => builder =>
    {
        builder.OpenComponent(0, typeof(CommonAncestors));
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
