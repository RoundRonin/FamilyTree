namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public enum AncestorAgeState
{
    ChooseFirst,
    ChooseSecond,
    View
}

public interface IAncestorAgeToolState : IToolState
{
    Queue<int> AncestorAgeCandidatesIds { get; }
    public AncestorAgeState State { get; set; }
}

