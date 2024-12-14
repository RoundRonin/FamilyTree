using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

namespace FamilyTreeBlazor.presentation.Services.Interfaces;

public interface IPersonRelationshipService
{
    public IPresentationService PresentationService { get; }

    public Person GetPerson(int id);

    public void AddPersonRelationship(Person person, Relationship rel, Relation type);
    public void AddRelationship(Relationship rel, Relation newRelation);
}