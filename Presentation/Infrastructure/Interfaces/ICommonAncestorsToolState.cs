namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public interface ICommonAncestorsToolState : IToolState
{
    Queue<int> CommonAncestorsCheckCandidatesIds { get; }
}

