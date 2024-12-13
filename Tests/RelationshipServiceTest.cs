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
}
