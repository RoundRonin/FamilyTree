using FamilyTreeBlazor.BLL.DTOs;

namespace FamilyTreeBlazor.BLL.Infrastructure;

public interface IPersonService
{
    Task<int> AddPersonAsync(PersonDTO person);
    PersonDTO? GetPersonById(int id);
    Task<IEnumerable<PersonDTO>> GetAllPersonsAsync();
    Task UpdatePersonAsync(PersonDTO person);
    Task DeletePersonAsync(int id);
    Task ClearAllDbAsync();
}
