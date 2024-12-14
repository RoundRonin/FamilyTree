using FamilyTreeBlazor.presentation.State.Interfaces;

namespace FamilyTreeBlazor.presentation.State.CommonAncestorsState.Interfaces;

public interface ICommonAncestorsToolState : IAppState 
{
    Queue<int> CommonAncestorsCheckCandidatesIds { get; }
}

public enum CommonAncestorsStateEnum
{
    ChooseFirst,
    ChooseSecond,
    View
}
