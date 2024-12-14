using FamilyTreeBlazor.presentation.Entities;

namespace FamilyTreeBlazor.presentation.Services.Interfaces;

public interface IRelationshipInfoService
{
    public IPresentationService PresentationService { get; }

    public IEnumerable<Person> GetChildren(int Id);
    public IEnumerable<Person> GetParents(int Id);
    public Person? GetSpouse(int Id);
}