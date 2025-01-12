using FamilyTreeBlazor.presentation.State.Interfaces;

namespace FamilyTreeBlazor.presentation.Services.Interfaces;

public interface IAncestorService
{
    public IPresentationService PresentationService { get; }

    public int? GetAncestorAge(int Id1, int Id2);
    public Dictionary<int, CardState> GetCommonAncestors(int Id1, int Id2);
    public Dictionary<int, CardState> GetPersonAncestors(int Id);
}