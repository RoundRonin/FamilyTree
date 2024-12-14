using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Controllers.Interfaces;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class CommonAncestorsToolState(IStateNotifier stateNotifier, IAncestorService ancestorService, IPersonRelationshipService personRelationshipService) : ToolStateBase(stateNotifier), ICommonAncestorsToolState
{
    private readonly Queue<int> _commonAncestorsCheckCandidatesIds = new();
    private Dictionary<int, CardState>? _commonAncestorsStates = new();
    public Queue<int> CommonAncestorsCheckCandidatesIds => _commonAncestorsCheckCandidatesIds;



    public CommonAncestorsState _state = CommonAncestorsState.ChooseFirst;
    public CommonAncestorsState State
    {
        get => _state;
        set
        {
            _state = value;
            NotifyStateChanged();
        }
    }

    public void AddCandidateId(int id)
    {
        bool enough = false;
        switch (_commonAncestorsCheckCandidatesIds.Count)
        {
            case 0:
                _state = CommonAncestorsState.ChooseSecond;
                break;
            case 1:
                if (id == _commonAncestorsCheckCandidatesIds.ElementAt(0)) return;
                _state = CommonAncestorsState.View;
                enough = true;
                break;
            case 2:
                if (id == _commonAncestorsCheckCandidatesIds.ElementAt(0)
                    || id == _commonAncestorsCheckCandidatesIds.ElementAt(1)) return;
                _commonAncestorsCheckCandidatesIds.Dequeue();
                enough = true;
                break;

            default:
                break;
        }

        _commonAncestorsCheckCandidatesIds.Enqueue(id);

        if (enough)
        {
            int id1 = _commonAncestorsCheckCandidatesIds.ElementAt(0);
            int id2 = _commonAncestorsCheckCandidatesIds.ElementAt(1);

            _commonAncestorsStates = ancestorService.GetCommonAncestors(id1, id2);

            Console.WriteLine("Hello, we are checking for ancestors");
            foreach (var (idx, state) in _commonAncestorsStates) Console.WriteLine(idx + " " + state);
        }

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
            case CommonAncestorsState.ChooseFirst:
                builder.OpenComponent(0, typeof(BlankPanel));
                builder.AddAttribute(1, "Header", "Common ancestors mode");
                builder.AddAttribute(2, "Text", "Choose the first person");
                builder.CloseComponent();
                break;
            case CommonAncestorsState.ChooseSecond:
                builder.OpenComponent(0, typeof(BlankPanel));
                builder.AddAttribute(1, "Header", "Common ancestors mode");
                builder.AddAttribute(2, "Text", "Choose the second person");
                builder.CloseComponent();
                break;
            case CommonAncestorsState.View:
                builder.OpenComponent(0, typeof(CommonAncestors));
                builder.AddAttribute(1, "PersonA", personRelationshipService.GetPerson(_commonAncestorsCheckCandidatesIds.ElementAt(0)));
                builder.AddAttribute(2, "PersonB", personRelationshipService.GetPerson(_commonAncestorsCheckCandidatesIds.ElementAt(1)));
                builder.AddAttribute(3, "Ancestors", _commonAncestorsStates.Keys.Select(id => personRelationshipService.GetPerson(id)));
                builder.CloseComponent();
                break;
            default:
                throw new NotImplementedException();
        }
    };

    public override RenderFragment RenderCard(Person person) => builder =>
    {
        CardState state = CardState.Default;
        int size = _commonAncestorsCheckCandidatesIds.Count;
        if (size >= 1 && person.Id == _commonAncestorsCheckCandidatesIds.ElementAt(0)) state = CardState.HighlightedChosen;
        if (size >= 2 && person.Id == _commonAncestorsCheckCandidatesIds.ElementAt(1)) state = CardState.HighlightedChosen;

        CardState state2;
        bool found = _commonAncestorsStates != null;
        if (found && _commonAncestorsStates.TryGetValue(person.Id, out state2))
        {
            state = state2;
        }

        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Person", person);
        builder.AddAttribute(2, "State", state);
        builder.CloseComponent();
    };

    public override object GetSpecificState() => this;
}
