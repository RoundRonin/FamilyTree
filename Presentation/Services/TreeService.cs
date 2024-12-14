using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using static FamilyTreeBlazor.presentation.Components.TreeView;

namespace FamilyTreeBlazor.presentation.Services;

class TreeService(ITreeCache treeCache, IStateNotifier stateNotifier) : ITreeService
{

    public ITreeCache CachedTree { get; private set; } = treeCache;
    public List<Person> _personList = [
        new(0, "Ivan Ivanov", DateTime.Now, true) { TreeDepth = 0, Links = { 1 } },
        new(1, "Anna Petrova", DateTime.Now, false) { TreeDepth = 1, Links = { 1, 2, 3 } },
        new(2, "Denis Pterov", DateTime.Now, true) { TreeDepth = 1, Links = { 2, 4 } },
        new(3, "Dasha Ivanova", DateTime.Now, false) { TreeDepth = 2, Links = { 3, 4, 11 } },
        new(4, "Dio Brando", DateTime.Now, true) { TreeDepth = 0, Links = { 5, 9 } },
        new(5, "Jorno Jovana", DateTime.Now, true) { TreeDepth = 1, Links = { 5, 6, 7, 10 } },
        new(6, "Ann Jovana", DateTime.Now, false) { TreeDepth = 1, Links = { 6, 8 } },
        new(7, "Kudjo Jotaro", DateTime.Now, true) { TreeDepth = 2, Links = { 7, 8, 11 } },
        new(8, "Katy Brando", DateTime.Now, false) { TreeDepth = 0, Links = { 9, 10 } },
    ];

    public List<Relationship> _relationshipList = [
        new(0, 1, RelationshipType.Parent, true),
        new(1, 2, RelationshipType.Spouse, true),
        new(1, 3, RelationshipType.Parent, true),
        new(2, 3, RelationshipType.Parent, true),
        new(4, 5, RelationshipType.Parent, true),
        new(5, 6, RelationshipType.Spouse, true),
        new(5, 7, RelationshipType.Parent, true),
        new(6, 7, RelationshipType.Parent, true),
        new(4, 8, RelationshipType.Spouse, true),
        new(8, 5, RelationshipType.Parent, true),
        new(3, 7, RelationshipType.Spouse, true),
    ];

    public List<Person> PersonList {  get => _personList; }
    public List<Relationship> RelationshipList {  get => _relationshipList; }

    public event Action OnDataChanged;

    public Person GetPerson(int Id)
    {
        Person person = _personList[Id];
        Console.WriteLine(person.Name);
        if (person == null) throw new Exception("relationship is broken");

        if (person.Id == Id) return person;

        _personList.Sort((x,y) => x.Id.CompareTo(y.Id));
        person = _personList[Id];

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

    public Person? GetSpouse(int Id)
    {
        // TODO use getImmediateRelatives
        throw new NotImplementedException();
    }

    public Dictionary<int, CardState> GetCommonAncestors(int Id1, int Id2)
    {

        throw new NotImplementedException();
    }

    public void AddPersonRelationship(Person person, Relationship rel, Relation type)
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
            case Relation.Child:
                depth++;
                break;
            case Relation.Parent:
                depth--;
                break;
            case Relation.Spouse:
                break;
            default:
                break;
        }
        // TEMP. Work with BLL
        person.Id = PersonList.Count;
        rel.PersonId2 = person.Id;

        int newRelIdx = RelationshipList.Count;
        RelationshipList.Add(rel);

        person.TreeDepth = depth;
        person.Links.Add(newRelIdx);
        oldPerson.Links.Add(newRelIdx);

        PersonList.Add(person);

        NotifyOnChanged();
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

        NotifyOnChanged();
    }

    private static bool ValidateRelationshipPerson(Person person, Relationship relationship)
    {
        if (person.Id != relationship.PersonId1 && person.Id != relationship.PersonId2)
        {
            return false;
        }

        return true;
    }

    private void NotifyOnChanged()
    {
        OnDataChanged?.Invoke();
    }

}