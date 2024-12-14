using FamilyTreeBlazor.BLL.DTOs;

namespace FamilyTreeBlazor.BLL.Infrastructure;

public interface IFamilyTreeService
{
    Task InitializeTreeAsync();
    IEnumerable<PersonDTO> GetImmediateRelatives(int personId);
    int CalculateAncestorAgeAtBirth(int ancestorId, int descendantId);
    Task ResetTreeAsync();
    IEnumerable<PersonDTO> FindCommonAncestors(int personId1, int personId2);
}
