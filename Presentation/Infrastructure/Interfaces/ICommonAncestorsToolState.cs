namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public enum CommonAncestorsState
{
    ChooseFirst,
    ChooseSecond,
    View
}
public interface ICommonAncestorsToolState : IToolState
{
    Queue<int> CommonAncestorsCheckCandidatesIds { get; }
    public CommonAncestorsState State { get; set; }
}

