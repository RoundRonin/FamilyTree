using FamilyTreeBlazor.BLL.DTOs;

namespace FamilyTreeBlazor.BLL.Infrastructure;

public interface IRelationshipService
{
    Task AddParentChildRelationshipAsync(int parentId, int childId);
    Task AddSpouseRelationshipAsync(int personId1, int personId2);
    Task<IEnumerable<RelationshipDTO>> GetAllRelationshipsAsync();
    Task ClearAllDbAsync();
}
