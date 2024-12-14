using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Services.Interfaces;

namespace FamilyTreeBlazor.presentation.Services;

public class RelationshipInfoService : IRelationshipInfoService
{
    private readonly IFamilyTreeService _familyTreeService;

    public IPresentationService PresentationService { get; private set; }

    public RelationshipInfoService(
        IFamilyTreeService familyTreeService,
        IPresentationService presentationService
        )
    {
        PresentationService = presentationService;
        _familyTreeService = familyTreeService;
    }

    public IEnumerable<Person> GetChildren(int Id)
    {
        return _familyTreeService.GetChildren(Id)
            .Where(personDTO => personDTO.Id.HasValue)
            .Select(personDTO => PresentationService.PersonDict[personDTO.Id.Value]);
    }

    public IEnumerable<Person> GetParents(int Id)
    {
        return _familyTreeService.GetParents(Id)
            .Where(personDTO => personDTO.Id.HasValue)
            .Select(personDTO => PresentationService.PersonDict[personDTO.Id.Value]);
    }

    public Person? GetSpouse(int Id)
    {
        var spouse = _familyTreeService.GetSpouse(Id);
        if (spouse == null || !spouse.Id.HasValue) return null;

        return PresentationService.PersonDict.TryGetValue(spouse.Id.Value, out var person) ? person : null;
    }
}