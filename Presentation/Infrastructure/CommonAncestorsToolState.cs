﻿using FamilyTreeBlazor.presentation.Components.Card;
using FamilyTreeBlazor.presentation.Components.DynamicPanel;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public class CommonAncestorsToolState(IStateNotifier stateNotifier, ITreeService treeService) : ToolStateBase(stateNotifier), ICommonAncestorsToolState
{
    private readonly Queue<int> _commonAncestorsCheckCandidatesIds = new();
    private Dictionary<int, CardState>? _commonAncestorsStates;
    public Queue<int> CommonAncestorsCheckCandidatesIds => _commonAncestorsCheckCandidatesIds;

    

    public CommonAncestorsState _state = CommonAncestorsState.ChooseFirst;
    public CommonAncestorsState State { 
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
        bool enough = false;
        if (_commonAncestorsCheckCandidatesIds.Count == 2)
        {
            _commonAncestorsCheckCandidatesIds.Dequeue(); 
            enough = true;
        }

        _commonAncestorsCheckCandidatesIds.Enqueue(id);

        if (enough)
        {
            int id1 = _commonAncestorsCheckCandidatesIds.ElementAt(0);
            int id2 = _commonAncestorsCheckCandidatesIds.ElementAt(1);

            //_commonAncestorsStates = treeService.GetCommonAncestors(id1, id2);
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
        if (found && _commonAncestorsStates.TryGetValue(person.Id, out state2)){
            state = state2;
        }

        builder.OpenComponent(0, typeof(PersonViewCard));
        builder.AddAttribute(1, "Name", person.Name);
        builder.AddAttribute(2, "BirthDay", person.BirthDateTime);
        builder.AddAttribute(3, "State", state);
        builder.CloseComponent();
    };

    public override object GetSpecificState() => this;
}
