using FamilyTreeBlazor.BLL;
using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.DAL.Entities;
using FamilyTreeBlazor.DAL.Infrastructure;
using Moq;
using Xunit;

namespace FamilyTreeBlazor.Tests;

public class RelationshipServiceTests
{
    private readonly Mock<IRepository<Relationship>> _relationshipRepositoryMock;
    private readonly TreeCacheDTO _treeCache;
    private readonly RelationshipService _relationshipService;

    public RelationshipServiceTests()
    {
        _relationshipRepositoryMock = new Mock<IRepository<Relationship>>();
        _treeCache = new TreeCacheDTO();
        _relationshipService = new RelationshipService(_relationshipRepositoryMock.Object, _treeCache);
    }

    // AddParentChildRelationshipAsync Tests
    [Fact]
    public async Task AddParentChildRelationshipAsync_ShouldAddRelationship()
    {
        // Arrange
        var parent = new PersonDTO(1, "Parent", new DateTime(1950, 1, 1), true);
        var child = new PersonDTO(2, "Child", new DateTime(1980, 1, 1), true);
        _treeCache.Persons[parent.Id] = parent;
        _treeCache.Persons[child.Id] = child;

        // Act
        await _relationshipService.AddParentChildRelationshipAsync(parent.Id, child.Id);

        // Assert
        _relationshipRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Relationship>()), Times.Once);
        Assert.Contains(child, parent.Children);
        Assert.Contains(parent, child.Parents);
    }

    [Fact]
    public async Task AddParentChildRelationshipAsync_WithMultipleRelations_ShouldAddAllRelationships()
    {
        // Arrange
        var parent1 = new PersonDTO(1, "Parent1", new DateTime(1950, 1, 1), true);
        var parent2 = new PersonDTO(2, "Parent2", new DateTime(1955, 1, 1), false);
        var child = new PersonDTO(3, "Child", new DateTime(1980, 1, 1), true);
        _treeCache.Persons[parent1.Id] = parent1;
        _treeCache.Persons[parent2.Id] = parent2;
        _treeCache.Persons[child.Id] = child;

        // Act
        await _relationshipService.AddParentChildRelationshipAsync(parent1.Id, child.Id);
        await _relationshipService.AddParentChildRelationshipAsync(parent2.Id, child.Id);

        // Assert
        _relationshipRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Relationship>()), Times.Exactly(2));
        Assert.Contains(child, parent1.Children);
        Assert.Contains(child, parent2.Children);
        Assert.Contains(parent1, child.Parents);
        Assert.Contains(parent2, child.Parents);
    }

    // AddSpouseRelationshipAsync Tests
    [Fact]
    public async Task AddSpouseRelationshipAsync_ShouldAddRelationship()
    {
        // Arrange
        var person1 = new PersonDTO(1, "Person1", new DateTime(1950, 1, 1), true);
        var person2 = new PersonDTO(2, "Person2", new DateTime(1950, 1, 1), false);
        _treeCache.Persons[person1.Id] = person1;
        _treeCache.Persons[person2.Id] = person2;

        // Act
        await _relationshipService.AddSpouseRelationshipAsync(person1.Id, person2.Id);

        // Assert
        _relationshipRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Relationship>()), Times.Once);
        Assert.Equal(person2, person1.Spouse);
        Assert.Equal(person1, person2.Spouse);
    }

    [Fact]
    public async Task AddSpouseRelationshipAsync_WithMultipleRelations_ShouldAddAllRelationships()
    {
        // Arrange
        var person1 = new PersonDTO(1, "Person1", new DateTime(1950, 1, 1), true);
        var person2 = new PersonDTO(2, "Person2", new DateTime(1950, 1, 1), false);
        var person3 = new PersonDTO(3, "Person3", new DateTime(1955, 1, 1), false);
        var person4 = new PersonDTO(4, "Person4", new DateTime(1955, 1, 1), true);
        _treeCache.Persons[person1.Id] = person1;
        _treeCache.Persons[person2.Id] = person2;
        _treeCache.Persons[person3.Id] = person3;
        _treeCache.Persons[person4.Id] = person4;

        // Act
        await _relationshipService.AddSpouseRelationshipAsync(person1.Id, person2.Id);
        await _relationshipService.AddSpouseRelationshipAsync(person3.Id, person4.Id);

        // Assert
        _relationshipRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Relationship>()), Times.Exactly(2));
        Assert.Equal(person2, person1.Spouse);
        Assert.Equal(person1, person2.Spouse);
        Assert.Equal(person4, person3.Spouse);
        Assert.Equal(person3, person4.Spouse);
    }

    // GetAllRelationshipsAsync Tests
    [Fact]
    public async Task GetAllRelationshipsAsync_ShouldReturnAllRelationships()
    {
        // Arrange
        var relationships = new List<Relationship>
        {
            //new Relationship(1, 1, 2, RelationshipType.Parent),
            //new Relationship(2, 2, 3, RelationshipType.Spouse)
            new Relationship(1, 2, DAL.Entities.RelationshipType.Parent),
            new Relationship(2, 3, DAL.Entities.RelationshipType.Spouse)
        };
        _relationshipRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(relationships);

        // Act
        var result = await _relationshipService.GetAllRelationshipsAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllRelationshipsAsync_EmptyDatabase_ShouldReturnEmpty()
    {
        // Arrange
        var relationships = new List<Relationship>();
        _relationshipRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(relationships);

        // Act
        var result = await _relationshipService.GetAllRelationshipsAsync();

        // Assert
        Assert.Empty(result);
    }

    // ClearAllDbAsync Tests
    [Fact]
    public async Task ClearAllDbAsync_ShouldClearAllRelationships()
    {
        // Act
        await _relationshipService.ClearAllDbAsync();

        // Assert
        _relationshipRepositoryMock.Verify(repo => repo.TruncateTableAsync(), Times.Once);
    }

    [Fact]
    public async Task ClearAllDbAsync_AfterMultipleOperations_ShouldClearAllRelationshipsFromDatabase()
    {
        // Arrange
        //var relationship1 = new Relationship(1, 1, 2, RelationshipType.Parent);
        //var relationship2 = new Relationship(2, 1, 3, RelationshipType.Spouse);
        var relationship1 = new Relationship(1, 2, DAL.Entities.RelationshipType.Parent);
        var relationship2 = new Relationship(1, 3, DAL.Entities.RelationshipType.Spouse);
        var relationships = new List<Relationship> { relationship1, relationship2 };
        _relationshipRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(relationships);
        
        // Act
        await _relationshipService.ClearAllDbAsync();
        
        // Mock repository to return empty list after truncation
        _relationshipRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Relationship>());

        var result = await _relationshipService.GetAllRelationshipsAsync();

        // Assert
        _relationshipRepositoryMock.Verify(repo => repo.TruncateTableAsync(), Times.Once);
        Assert.Empty(result);
    }
}
