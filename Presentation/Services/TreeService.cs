using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.BLL;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.BLL.DTOs;
using RelationshipType = FamilyTreeBlazor.presentation.Entities.RelationshipType;

namespace FamilyTreeBlazor.presentation.Services;

class TreeService() : ITreeService
{
    private IFamilyTreeService _familyTreeService;
    private IPersonService _personService;
    private IRelationshipService _relationshipService;

    public ITreeCache CachedTree { get; private set; }

    // Replace List<Person> with Dictionary<int, Person>
    public Dictionary<int, Person> _personDict = new();
    public List<Relationship> _relationshipList = new();
        
    public List<Person> PersonList 
    {  
        get 
        { 
            var sortedList = _personDict.Values.ToList();
            sortedList.Sort((x, y) => x.Id.CompareTo(y.Id));
            return sortedList;
        } 
    }
    public List<Relationship> RelationshipList { get => _relationshipList; }

    public event Action OnDataChanged;

    public TreeService(
        ITreeCache treeCache,
        IFamilyTreeService familyTreeService,
        IPersonService personService,
        IRelationshipService relationshipService
        ) : this()
    {
        CachedTree = treeCache;
        _familyTreeService = familyTreeService;
        _personService = personService;
        _relationshipService = relationshipService;

        InitializeTree();
    }

    public Person GetPerson(int Id)
    {
        if (_personDict.TryGetValue(Id, out Person? person))
        {
            Console.WriteLine(person.Name);
            return person;
        }
        throw new Exception("Unable to find person with the given Id");
    }

    public void UpdateTree()
    {
        throw new NotImplementedException();
    }

    public async void DeleteTree() 
    {
        await _familyTreeService.ResetTreeAsync();

        _relationshipList.Clear();
        _personDict.Clear();

        NotifyOnChanged();
    }

    public IEnumerable<Person> GetChildren(int Id)
    {
        return _familyTreeService.GetChildren(Id)
            .Where(personDTO => personDTO.Id.HasValue)
            .Select(personDTO => _personDict[personDTO.Id.Value]);
    }

    public IEnumerable<Person> GetParents(int Id)
    {
        return _familyTreeService.GetParents(Id)
            .Where(personDTO => personDTO.Id.HasValue)
            .Select(personDTO => _personDict[personDTO.Id.Value]);
    }

    public Person? GetSpouse(int Id)
    {
        var spouse = _familyTreeService.GetSpouse(Id);
        if (spouse == null || !spouse.Id.HasValue) return null;

        return _personDict.TryGetValue(spouse.Id.Value, out var person) ? person : null;
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
        if (!_personDict.TryGetValue(rel.GetExistingPersonId(), out Person? oldPerson))
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
        _relationshipList.Add(rel);
        _personDict[person.Id] = person;

        NotifyOnChanged();
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
        _relationshipList.Add(rel);

        NotifyOnChanged();
    }

    private static bool ValidateRelationshipPerson(Person person, Relationship relationship)
    {
        return person.Id == relationship.PersonId1 || person.Id == relationship.PersonId2;
    }

    private void NotifyOnChanged()
    {
        OnDataChanged?.Invoke();
    }

    private async void InitializeTree()
    {
        // Traverse the tree!
        await _familyTreeService.InitializeTreeAsync();
        Queue<KeyValuePair<PersonDTO, int>> q = new();

        if (CachedTree.Persons.Count == 0)
        {
            await _personService.AddPersonAsync(new("Default", DateTime.Now, true));
        }
        var personPair = CachedTree.Persons.First();
        var personDTO = personPair.Value;

        int depth = 0;
        q.Clear();
        q.Enqueue(new(personDTO, 0));
        HashSet<int> visited = [];
        while (q.Count > 0)
        {
            (personDTO, depth) = q.Dequeue();
            if (personDTO.Id == null) throw new Exception("Id is not set");
            visited.Add(personDTO.Id.Value);

            foreach (var child in personDTO.Children)
            {
                if (child == null || visited.Contains(child.Id.Value)) continue;
                q.Enqueue(new(child, depth + 1));
                Relationship rel = new(personDTO.Id.Value, child.Id.Value, RelationshipType.Parent, true);
                _relationshipList.Add(rel);
            }

            foreach (var parent in personDTO.Parents)
            {
                if (parent == null || visited.Contains(parent.Id.Value)) continue;
                q.Enqueue(new(parent, depth - 1));
                Relationship rel = new(parent.Id.Value, personDTO.Id.Value, RelationshipType.Parent, true);
                _relationshipList.Add(rel);
            }

            if (personDTO.Spouse != null && !visited.Contains(personDTO.Spouse.Id.Value))
            {
                q.Enqueue(new(personDTO.Spouse, depth));
                Relationship rel = new(personDTO.Id.Value, personDTO.Spouse.Id.Value, RelationshipType.Spouse, true);
                _relationshipList.Add(rel);
            }

            Person person = new(personDTO.Id.Value, personDTO.Name, personDTO.BirthDateTime.ToUniversalTime(), personDTO.Sex)
            {
                TreeDepth = depth
            };
            _personDict[person.Id] = person;
        }
    }
}
