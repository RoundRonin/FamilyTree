using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.BLL.Infrastructure;

namespace FamilyTreeBlazor.BLL;

public class FamilyTreeService(IPersonService personService, IRelationshipService relationshipService, ITreeCache treeCache) : IFamilyTreeService
{
    private readonly IPersonService _personService = personService;
    private readonly IRelationshipService _relationshipService = relationshipService;
    private readonly ITreeCache _treeCache = treeCache;

    public async Task InitializeTreeAsync()
    {
        var persons = await _personService.GetAllPersonsAsync();
        var relationships = await _relationshipService.GetAllRelationshipsAsync();

        foreach (var person in persons)
        {
            if (person.Id == null) throw new Exception("Person has no Id.. Internal error");
            _treeCache.Persons[person.Id.Value] = person;
        }

        foreach (var relationship in relationships)
        {
            if (_treeCache.Persons.TryGetValue(relationship.PersonId1, out var person1) &&
                _treeCache.Persons.TryGetValue(relationship.PersonId2, out var person2))
            {
                if (relationship.RelationshipType == RelationshipType.Parent)
                {
                    person1.Children.Add(person2);
                    person2.Parents.Add(person1);
                }
                else if (relationship.RelationshipType == RelationshipType.Spouse)
                {
                    person1.Spouse = person2;
                    person2.Spouse = person1;
                }
            }
        }
    }

    public IEnumerable<PersonDTO> GetImmediateRelatives(int personId)
    {
        if (!_treeCache.Persons.TryGetValue(personId, out var person))
        {
            return [];
        }

        List<PersonDTO> relatives = [];
        relatives.AddRange(person.Parents);
        relatives.AddRange(person.Children);

        return relatives;
    }

    public int CalculateAncestorAgeAtBirth(int ancestorId, int descendantId)
    {
        if (!_treeCache.Persons.TryGetValue(ancestorId, out var ancestor) ||
            !_treeCache.Persons.TryGetValue(descendantId, out var descendant))
        {
            throw new Exception("Person not found");
        }

        if (!IsDescendant(ancestor, descendant))
        {
            throw new InvalidOperationException("The specified person is not a descendant of the ancestor.");
        }

        return descendant.BirthDateTime.Year - ancestor.BirthDateTime.Year;
    }

    // This method is a bruteforce solution. Testing is needed to evaluate performance and change it potentially
    public IEnumerable<PersonDTO> FindCommonAncestors(int personId1, int personId2)
    {
        if (!_treeCache.Persons.TryGetValue(personId1, out var person1) ||
            !_treeCache.Persons.TryGetValue(personId2, out var person2))
        {
            throw new Exception("Person not found");
        }

        var ancestors1 = GetAllAncestors(person1);
        var ancestors2 = GetAllAncestors(person2);

        return ancestors1.Intersect(ancestors2);
    }

    public async Task ResetTreeAsync()
    {
        await _personService.ClearAllDbAsync();
        await _relationshipService.ClearAllDbAsync();
        _treeCache.Persons.Clear();
    }

    private static bool IsDescendant(PersonDTO ancestor, PersonDTO descendant)
    {
        foreach (var parent in descendant.Parents)
        {
            if (parent.Id == ancestor.Id || IsDescendant(ancestor, parent))
            {
                return true;
            }
        }
        return false;
    }
    private static HashSet<PersonDTO> GetAllAncestors(PersonDTO person)
    {
        var ancestors = new HashSet<PersonDTO>();

        foreach (var parent in person.Parents)
        {
            ancestors.Add(parent);
            ancestors.UnionWith(GetAllAncestors(parent));
        }

        return ancestors;
    }
}
