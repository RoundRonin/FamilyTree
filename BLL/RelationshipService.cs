using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.DAL.Infrastructure;
using FamilyTreeBlazor.DAL.Entities;
using FamilyTreeBlazor.BLL.Infrastructure;

namespace FamilyTreeBlazor.BLL;

public class RelationshipService(IRepository<Relationship> relationshipRepository, TreeCacheDTO treeCache) : IRelationshipService
{
    private readonly IRepository<Relationship> _relationshipRepository = relationshipRepository;
    private readonly TreeCacheDTO _treeCache = treeCache;

    public async Task AddParentChildRelationshipAsync(int parentId, int childId)
    {
        if (!_treeCache.Persons.TryGetValue(parentId, out var parent))
        {
            throw new InvalidOperationException($"Parent with ID {parentId} not found.");
        }

        if (!_treeCache.Persons.TryGetValue(childId, out var child))
        {
            throw new InvalidOperationException($"Child with ID {childId} not found.");
        }

        var relationship = new Relationship(parentId, childId, DAL.Entities.RelationshipType.Parent);
        
        await _relationshipRepository.AddAsync(relationship);

        parent.Children.Add(child);
        child.Parents.Add(parent);
    }

    public async Task AddSpouseRelationshipAsync(int personId1, int personId2)
    {
        if (!_treeCache.Persons.TryGetValue(personId1, out var person1))
        {
            throw new InvalidOperationException($"Person with ID {personId1} not found.");
        }

        if (!_treeCache.Persons.TryGetValue(personId2, out var person2))
        {
            throw new InvalidOperationException($"Person with ID {personId2} not found.");
        }

        var relationship = new Relationship(personId1, personId2, DAL.Entities.RelationshipType.Spouse);
        
        await _relationshipRepository.AddAsync(relationship);

        person1.Spouse = person2;
        person2.Spouse = person1;
    }

    public async Task<IEnumerable<RelationshipDTO>> GetAllRelationshipsAsync() 
    { 
        var persons = await _relationshipRepository.GetAllAsync();

        return persons.Select(person => new RelationshipDTO(
            person.Id, 
            person.PersonId1,
            person.PersonId2,
            (DTOs.RelationshipType)person.RelationshipType)
        );
    }

    public async Task ClearAllDbAsync()
    {
        await _relationshipRepository.TruncateTableAsync();
    }
}
