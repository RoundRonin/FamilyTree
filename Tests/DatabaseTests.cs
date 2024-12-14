using FamilyTreeBlazor.DAL.Infrastructure;
using FamilyTreeBlazor.DAL.Entities;
using FamilyTreeBlazor.DAL;
using FamilyTreeBlazor.BLL;
using FamilyTreeBlazor.BLL.DTOs;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FamilyTreeBlazor.Tests;

public class InMemoryDatabaseTests : IDisposable
{
    private readonly ApplicationContext _context;

    public InMemoryDatabaseTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: "TestInMemoryDatabase")
            .Options;

        _context = new ApplicationContext(options);
    }

    [Fact]
    public async Task AddPerson_ShouldAddPersonToDatabase()
    {
        var repository = new Repository<Person>(_context);
        var personService = new PersonService(repository, new TreeCacheDTO());
        var personDto = new PersonDTO ("John Doe", new DateTime(1980, 1, 1), true);

        await personService.AddPersonAsync(personDto);

        var personInDb = await _context.Persons.FindAsync(personDto.Id);
        Assert.NotNull(personInDb);
        Assert.Equal("John Doe", personInDb.Name);
    }

    [Fact]
    public async Task GetAllPersons_ShouldReturnAllPersons()
    {
        var repository = new Repository<Person>(_context);
        var personService = new PersonService(repository, new TreeCacheDTO());
        var person1 = new Person("John Doe", new DateTime(1980, 1, 1), true);
        var person2 = new Person("Jane Doe", new DateTime(1985, 1, 1), false);

        _context.Persons.AddRange(person1, person2);
        await _context.SaveChangesAsync();

        var persons = await personService.GetAllPersonsAsync();
        Assert.Equal(2, persons.Count());
    }

    [Fact]
    public async Task AddParentChildRelationship_ShouldAddRelationshipToDatabase()
    {
        var relRepo = new Repository<Relationship>(_context);
        var perRepo = new Repository<Person>(_context);

        var tree = new TreeCacheDTO();
        var relationshipService = new RelationshipService(relRepo, tree);
        var personService = new PersonService(perRepo, tree);
        var parent = new PersonDTO("Parent", new DateTime(1980, 1, 1), true);
        var child = new PersonDTO("Child", new DateTime(1985, 1, 1), false);

        await personService.AddPersonAsync(parent);
        await personService.AddPersonAsync(child);

        var persons = await personService.GetAllPersonsAsync();

        await relationshipService.AddParentChildRelationshipAsync(persons.ElementAt(0).Id.Value, persons.ElementAt(1).Id.Value);

        var relationshipInDb = await _context.Relationships.FirstOrDefaultAsync(r => 
            r.PersonId1 == parent.Id && r.PersonId2 == child.Id && r.RelationshipType == DAL.Entities.RelationshipType.Parent);
        Assert.NotNull(relationshipInDb);
        Assert.Equal(DAL.Entities.RelationshipType.Parent, relationshipInDb.RelationshipType);
    }

    [Fact]
    public async Task AddSpouseRelationship_ShouldAddRelationshipToDatabase()
    {
        var relRepo = new Repository<Relationship>(_context);
        var perRepo = new Repository<Person>(_context);

        var tree = new TreeCacheDTO();
        var relationshipService = new RelationshipService(relRepo, tree);
        var personService = new PersonService(perRepo, tree);
        var person1 = new PersonDTO("Person1", new DateTime(1980, 1, 1), true);
        var person2 = new PersonDTO("Person2", new DateTime(1985, 1, 1), false);

        await personService.AddPersonAsync(person1);
        await personService.AddPersonAsync(person2);

        var persons = await personService.GetAllPersonsAsync();

        await relationshipService.AddSpouseRelationshipAsync(persons.ElementAt(0).Id.Value, persons.ElementAt(1).Id.Value);

        var relationshipInDb = await relRepo.GetAllAsync();
        Assert.NotNull(relationshipInDb.ElementAt(0));
        Assert.Equal(DAL.Entities.RelationshipType.Spouse, relationshipInDb.ElementAt(0).RelationshipType);
        Assert.Single(relationshipInDb);
    }

    [Fact]
    public async Task GetAllRelationships_ShouldReturnAllRelationships()
    {
        var repository = new Repository<Relationship>(_context);
        var relationshipService = new RelationshipService(repository, new TreeCacheDTO());
        var relationship1 = new Relationship(1, 2, DAL.Entities.RelationshipType.Parent);
        var relationship2 = new Relationship(2, 3, DAL.Entities.RelationshipType.Spouse);

        _context.Relationships.AddRange(relationship1, relationship2);
        await _context.SaveChangesAsync();

        var relationships = await relationshipService.GetAllRelationshipsAsync();
        Assert.Equal(2, relationships.Count());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
