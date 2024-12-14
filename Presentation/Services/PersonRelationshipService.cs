using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;

namespace FamilyTreeBlazor.presentation.Services;

public class PersonRelationshipService : IPersonRelationshipService
{
    private IPersonService _personService;
    private IRelationshipService _relationshipService;

    public IPresentationService PresentationService { get; private set; }

    public PersonRelationshipService(
        IPresentationService presentationService,
        IPersonService personService,
        IRelationshipService relationshipService
        )
    {
        PresentationService = presentationService;
        _personService = personService;
        _relationshipService = relationshipService;
    }

    public Person GetPerson(int Id)
    {
        if (PresentationService.PersonDict.TryGetValue(Id, out Person? person))
        {
            Console.WriteLine(person.Name);
            return person;
        }
        throw new Exception("Unable to find person with the given Id");
    }

    public async void AddPersonRelationship(Person person, Relationship rel, Relation newRelation)
    {
        // check
        if (!ValidateRelationshipPerson(person, rel)) throw new Exception("Relationship is broken");

        // BLL LOGIC
        PersonDTO personDTO = new(person.Name, person.BirthDateTime.ToUniversalTime(), person.Sex);
        int id = await _personService.AddPersonAsync(personDTO);
        person.Id = id;

        // Act
        // Find the person
        if (!PresentationService.PersonDict.TryGetValue(rel.GetExistingPersonId(), out Person? oldPerson))
        {
            throw new Exception("Such a person doesn't exist");
        }
        int depth = oldPerson.TreeDepth;

        rel.PersonId2 = person.Id;
        switch (newRelation)
        {
            case Relation.Child:
                depth++;
                await _relationshipService.AddParentChildRelationshipAsync(rel.PersonId1, rel.PersonId2);
                break;
            case Relation.Parent:
                depth--;
                await _relationshipService.AddParentChildRelationshipAsync(rel.PersonId2, rel.PersonId1);
                break;
            case Relation.Spouse:
                await _relationshipService.AddSpouseRelationshipAsync(rel.PersonId1, rel.PersonId2);
                break;
            default:
                break;
        }

        person.TreeDepth = depth;
        PresentationService.RelationshipList.Add(rel);
        PresentationService.PersonDict[person.Id] = person;

        PresentationService.NotifyOnChanged();
    }

    public async void AddRelationship(Relationship rel, Relation newRelation)
    {
        // BLL LOGIC
        switch (newRelation)
        {
            case Relation.Child:
                await _relationshipService.AddParentChildRelationshipAsync(rel.PersonId1, rel.PersonId2);
                break;
            case Relation.Parent:
                await _relationshipService.AddParentChildRelationshipAsync(rel.PersonId2, rel.PersonId1);
                break;
            case Relation.Spouse:
                await _relationshipService.AddSpouseRelationshipAsync(rel.PersonId1, rel.PersonId2);
                break;
            default:
                break;
        }

        // ACT
        PresentationService.RelationshipList.Add(rel);

        PresentationService.NotifyOnChanged();
    }

    private static bool ValidateRelationshipPerson(Person person, Relationship relationship)
    {
        return person.Id == relationship.PersonId1 || person.Id == relationship.PersonId2;
    }
}