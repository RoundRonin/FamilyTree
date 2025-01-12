using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using RelationshipType = FamilyTreeBlazor.presentation.Models.RelationshipType;

namespace FamilyTreeBlazor.presentation.Services;

class PresentationService() : IPresentationService
{
    private readonly IFamilyTreeService _familyTreeService;
    private readonly IPersonService _personService;

    public ITreeCache CachedTree { get; private set; }

    public Dictionary<int, Person> _personDict = [];
    public List<Relationship> _relationshipList = [];

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
    public Dictionary<int, Person> PersonDict { get => _personDict; }

    public event Action? OnDataChanged;

    public PresentationService(
        IFamilyTreeService familyTreeService,
        IPersonService personService,
        ITreeCache treeCache
        ) : this()
    {
        _familyTreeService = familyTreeService;
        _personService = personService;
        CachedTree = treeCache;

        InitializeTree();
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

    public async void AddInitialPerson(Person person)
    {
        // BLL LOGIC
        PersonDTO personDTO = new(person.Name, person.BirthDateTime.ToUniversalTime(), person.Sex);
        int id = await _personService.AddPersonAsync(personDTO);
        person.Id = id;

        // TreeService logic
        person.TreeDepth = 0;
        _personDict[person.Id] = person;

        NotifyOnChanged();
    }

    public void NotifyOnChanged()
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
            return;
            await _personService.AddPersonAsync(new("Default", DateTime.Now, true)); // TODO remove
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