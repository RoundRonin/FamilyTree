using FamilyTreeBlazor.BLL.DTOs;

namespace FamilyTreeBlazor.BLL.Infrastructure;

public interface ITreeCache
{
    public Dictionary<int, PersonDTO> Persons { get; set; } 
}
