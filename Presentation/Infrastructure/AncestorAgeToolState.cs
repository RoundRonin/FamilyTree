using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class AncestorAgeToolState(IStateNotifier stateNotifier) : ToolStateBase(stateNotifier), IAncestorAgeToolState
{
    private readonly Queue<int> _ancestorAgeCandidatesIds = new();
    public Queue<int> AncestorAgeCandidatesIds => _ancestorAgeCandidatesIds;


    public AncestorAgeState _state = AncestorAgeState.ChooseFirst;
    public AncestorAgeState State {
        get => _state;
        set
        {
            _state = value;
            NotifyStateChanged();
        }
    }

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

    public override void HandleId(int Id)
    {
        AddCandidateId(Id);
    }

    public override RenderFragment RenderPanel() => builder =>
    {
        switch (_state)
        {
            case AncestorAgeState.ChooseFirst:
                builder.OpenComponent(0, typeof(BlankPanel));
                builder.AddAttribute(1, "Header", "Ancestor age mode");
                builder.AddAttribute(2, "Text", "Choose a person");
                builder.CloseComponent();
                break;
            case AncestorAgeState.ChooseSecond:
                builder.OpenComponent(0, typeof(BlankPanel));
                builder.AddAttribute(1, "Header", "Ancestor age mode");
                builder.AddAttribute(2, "Text", "Choose an ancestor");
                builder.CloseComponent();
                break;
            case AncestorAgeState.View:
                builder.OpenComponent(0, typeof(AncestorAge));
                builder.CloseComponent();
                break;
            default:
                throw new NotImplementedException();
        }
    };

    public override RenderFragment RenderCard(Person person) => builder =>
    {
        CardState state = CardState.Default;
        int size = _ancestorAgeCandidatesIds.Count;
        if (size >= 1 && person.Id == _ancestorAgeCandidatesIds.ElementAt(0)) state = CardState.HighlightedChosen;
        else if (size >= 2 && person.Id == _ancestorAgeCandidatesIds.ElementAt(1)) state = CardState.HighlightedChosen;

        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Name", person.Name);
        builder.AddAttribute(2, "BirthDay", person.BirthDateTime);
        builder.AddAttribute(3, "State", state);
        builder.CloseComponent();
    };

    public override object GetSpecificState() => this;
}
