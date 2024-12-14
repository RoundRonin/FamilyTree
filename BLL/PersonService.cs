using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.DAL.Infrastructure;
using FamilyTreeBlazor.DAL.Entities;
using FamilyTreeBlazor.BLL.Infrastructure;

namespace FamilyTreeBlazor.BLL;

public class PersonService(IRepository<Person> personRepository, ITreeCache treeCache) : IPersonService
{
    private readonly IRepository<Person> _personRepository = personRepository;
    private readonly ITreeCache _treeCache = treeCache;

    public async Task AddPersonAsync(PersonDTO person)
    {
        var entity = new Person(person.Name, person.BirthDateTime, person.Sex);
        await _personRepository.AddAsync(entity);

        person.Id = entity.Id;
        if (person.Id == null)
        {
            throw new Exception("Id is not set.. Internal error");
        }

        _treeCache.Persons[person.Id.Value] = person;
    }

    public PersonDTO? GetPersonById(int id)
    {
        return _treeCache.Persons.TryGetValue(id, out var person) ? person : null;
    }

    public async Task<IEnumerable<PersonDTO>> GetAllPersonsAsync() 
    { 
        var persons = await _personRepository.GetAllAsync();

        return persons.Select(person => new PersonDTO(person.Name, person.BirthDateTime, person.Sex, person.Id));
    }

    public async Task UpdatePersonAsync(PersonDTO person)
    {
        if (!person.Id.HasValue) 
        { 
            throw new ArgumentException("ID must be provided for update operations."); 
        }

        var existingEntity = await _personRepository.RetrieveByIdAsync(person.Id.Value);
        if (existingEntity == null)
        {
            throw new InvalidOperationException($"Person with ID {person.Id} does not exist.");
        }

        existingEntity.Name = person.Name;
        existingEntity.BirthDateTime = person.BirthDateTime;
        existingEntity.Sex = person.Sex;
        
        await _personRepository.UpdateAsync(existingEntity);
        _treeCache.Persons[person.Id.Value] = person;
    }

    public async Task DeletePersonAsync(int id)
    {
        await _personRepository.DeleteAsync(id);
        _treeCache.Persons.Remove(id);
    }

    public async Task ClearAllDbAsync()
    {
        await _personRepository.TruncateTableAsync();
    }
        
}
