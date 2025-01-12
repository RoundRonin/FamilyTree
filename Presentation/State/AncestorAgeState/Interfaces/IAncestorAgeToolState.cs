using FamilyTreeBlazor.presentation.State.Interfaces;

namespace FamilyTreeBlazor.presentation.State.AncestorAgeState.Interfaces;

public enum AncestorAgeStateEnum
{
    ChooseFirst,
    ChooseSecond,
    View
}

public interface IAncestorAgeToolState : IAppState 
{
    Queue<int> AncestorAgeCandidatesIds { get; }
}
