using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using static FamilyTreeBlazor.presentation.Components.TreeView;

namespace FamilyTreeBlazor.presentation.Services;

class TreeService(ITreeCache treeCache) : ITreeService
{
    public ITreeCache CachedTree { get; private set; } = treeCache;
    public List<Person> _personList = [
        new(1, "Ivan Ivanov", DateTime.Now, true) { TreeDepth = 0 },
        new(2, "Anna Petrova", DateTime.Now, false) { TreeDepth = 1 },
        new(3, "Denis Pterov", DateTime.Now, true) { TreeDepth = 1 },
        new(4, "Dasha Ivanova", DateTime.Now, false) { TreeDepth = 2 },
        new(5, "Dio Brando", DateTime.Now, true) { TreeDepth = 0 },
        new(6, "Jorno Jovana", DateTime.Now, true) { TreeDepth = 1 },
        new(7, "Ann Jovana", DateTime.Now, false) { TreeDepth = 1 },
        new(8, "Kudjo Jotaro", DateTime.Now, true) { TreeDepth = 2 },
        new(9, "Katy Brando", DateTime.Now, false) { TreeDepth = 0 },
    ];
    public List<Relationship> _relationshipList = [
        new(1, 2, RelationshipType.Parent, true),
        new(2, 3, RelationshipType.Spouse, true),
        new(2, 4, RelationshipType.Parent, true),
        new(3, 4, RelationshipType.Parent, true),
        new(5, 6, RelationshipType.Parent, true),
        new(6, 7, RelationshipType.Spouse, true),
        new(6, 8, RelationshipType.Parent, true),
        new(7, 8, RelationshipType.Parent, true),
        new(5, 9, RelationshipType.Spouse, true),
        new(9, 6, RelationshipType.Parent, true),
        new(4, 8, RelationshipType.Spouse, true),
    ];

    public List<Person> PersonList {  get => _personList; }
    public List<Relationship> RelationshipList {  get => _relationshipList; }

    public Person GetPerson(int Id)
    {
        int IdFixed = Id - 1;

        Person person = _personList[IdFixed];
        Console.WriteLine(person.Name);
        if (person == null) throw new Exception("relationship is broken");

        if (person.Id == Id) return person;

        _personList.Sort((x,y) => x.Id.CompareTo(y.Id));
        person = _personList[IdFixed];

        if (person.Id == Id) return person;

        throw new Exception("Unable to sort the list");
    }

    public void InitializeTree()
    {
        throw new NotImplementedException();
    }

    public void UpdateTree()
    {
        throw new NotImplementedException();
    }

    public void DeleteTree() { 

        // ADD BLL LOGIC

        RelationshipList.Clear();
        PersonList.Clear();
    }

    public Person FindPerson(int Id)
    {
        return PersonList.Find(person => person.Id == Id) ?? throw new Exception("Such a person doesn't exist");
    }
    public BLL.DTOs.PersonListDTO GetKids(int Id)
    {
        // TODO use getImmediateRelatives
        throw new NotImplementedException();
    }

    public BLL.DTOs.PersonListDTO GetParents(int Id)
    {
        // TODO use getImmediateRelatives
        throw new NotImplementedException();
    }

    public Person GetSpouse(int Id)
    {
        // TODO use getImmediateRelatives
        throw new NotImplementedException();
    }

    public Dictionary<int, CardState> GetCommonAncestors(int Id1, int Id2)
    {

        throw new NotImplementedException();
    }

    public void AddPersonRelationship(Person person, Relationship rel, InsertionType type)
    {
        // check
        if (!ValidateRelationshipPerson(person, rel)) throw new Exception("relationship is broken");

        // ADD BLL LOGIC

        // Act
        // Find the person
        Person oldPerson = PersonList.Find(person => person.Id == rel.GetExistingPersonId()) ?? throw new Exception("Such a person doesn't exist");
        int depth = oldPerson.TreeDepth;

        switch (type)
        {
            case InsertionType.child:
                depth--;
                break;
            case InsertionType.parent:
                depth++;
                break;
            case InsertionType.spouse:
                break;
            default:
                break;
        }

        RelationshipList.Add(rel);
        int newRelIdx = RelationshipList.Count - 1;

        person.TreeDepth = depth;
        person.Links.Add(newRelIdx);
        oldPerson.Links.Add(newRelIdx);

        // TEMP. Work with BLL
        person.Id = PersonList.Last().Id + 1;
        PersonList.Add(person);
    }

    public void AddRelationship(Relationship rel)
    {
        // ADD BLL LOGIC

        Person P1 = PersonList.Find(p => p.Id == rel.PersonId1) ?? throw new Exception("Such a person doesn't exist");;
        Person P2 = PersonList.Find(p => p.Id == rel.PersonId2) ?? throw new Exception("Such a person doesn't exist");;

        RelationshipList.Add(rel);
        int newRelIdx = RelationshipList.Count - 1;

        P1.Links.Add(newRelIdx);
        P2.Links.Add(newRelIdx);
    }

    private static bool ValidateRelationshipPerson(Person person, Relationship relationship)
    {
        if (person.Id != relationship.PersonId1 && person.Id != relationship.PersonId2)
        {
            return false;
        }

        return true;
    }
}