using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Services.Interfaces;

namespace FamilyTreeBlazor.presentation.Services;

public class AncestorService : IAncestorService
{
    private IFamilyTreeService _familyTreeService;

    public IPresentationService PresentationService { get; private set; }

    public AncestorService(
        IPresentationService presentationService,
        IFamilyTreeService familyTreeService
        )
    {
        PresentationService = presentationService;
        _familyTreeService = familyTreeService;
    }

    public int? GetAncestorAge(int ancestor, int descendant)
    {
        int age;
        try
        {
            age = _familyTreeService.CalculateAncestorAgeAtBirth(ancestor, descendant);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
        return age;
    }

    public Dictionary<int, CardState> GetCommonAncestors(int Id1, int Id2)
    {
        return _familyTreeService.FindCommonAncestors(Id1, Id2).ToDictionary(
            p => p.Id.Value, p => CardState.HighlightedCommonAncestor);
    }
    public Dictionary<int, CardState> GetPersonAncestors(int Id)
    {
        return _familyTreeService.GetPersonAncestors(Id).ToDictionary(
            p => p.Id.Value, p => CardState.HighlightedCommonAncestor);
    }
}