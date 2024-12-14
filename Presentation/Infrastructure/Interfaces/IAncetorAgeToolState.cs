namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public interface IAncestorAgeToolState : IToolState
{
    Queue<int> AncestorAgeCandidatesIds { get; }
}

