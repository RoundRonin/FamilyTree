using FamilyTreeBlazor.BLL.Infrastructure;

namespace FamilyTreeBlazor.BLL.DTOs;

public class TreeCacheDTO : ITreeCache
{
    public Dictionary<int, PersonDTO> Persons { get; set; } = [];
}
