﻿using FamilyTreeBlazor.BLL;
using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.BLL.Infrastructure;
using Moq;
using Xunit;

namespace FamilyTreeBlazor.Tests;

public class FamilyTreeServiceTests
{
    private readonly Mock<IPersonService> _personServiceMock;
    private readonly Mock<IRelationshipService> _relationshipServiceMock;
    private readonly TreeCacheDTO _treeCache;
    private readonly FamilyTreeService _familyTreeService;

    public FamilyTreeServiceTests()
    {
        _personServiceMock = new Mock<IPersonService>();
        _relationshipServiceMock = new Mock<IRelationshipService>();
        _treeCache = new TreeCacheDTO();
        _familyTreeService = new FamilyTreeService(_personServiceMock.Object, _relationshipServiceMock.Object, _treeCache);
    }

    // InitializeTreeAsync Tests
    [Fact]
    public async Task InitializeTreeAsync_ShouldInitializeTree()
    {
        // Arrange
        var persons = new List<PersonDTO>
        {
            new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true)
        };
        var relationships = new List<RelationshipDTO>();

        _personServiceMock.Setup(service => service.GetAllPersonsAsync()).ReturnsAsync(persons);
        _relationshipServiceMock.Setup(service => service.GetAllRelationshipsAsync()).ReturnsAsync(relationships);

        // Act
        await _familyTreeService.InitializeTreeAsync();

        // Assert
        Assert.True(_treeCache.Persons.ContainsKey(1));
    }

    [Fact]
    public async Task InitializeTreeAsync_WithMultiplePersonsAndRelationships_ShouldInitializeTreeCorrectly()
    {
        // Arrange
        var persons = new List<PersonDTO>
        {
            new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true),
            new PersonDTO(2, "Jane Doe", new DateTime(1985, 1, 1), false),
            new PersonDTO(3, "Child1", new DateTime(2010, 1, 1), true),
            new PersonDTO(4, "Child2", new DateTime(2015, 1, 1), false)
        };
        var relationships = new List<RelationshipDTO>
        {
            new RelationshipDTO(1, 1, 3, RelationshipType.Parent),
            new RelationshipDTO(2, 1, 4, RelationshipType.Parent),
            new RelationshipDTO(3, 2, 3, RelationshipType.Parent),
            new RelationshipDTO(4, 2, 4, RelationshipType.Parent),
            new RelationshipDTO(5, 1, 2, RelationshipType.Spouse)
        };

        _personServiceMock.Setup(service => service.GetAllPersonsAsync()).ReturnsAsync(persons);
        _relationshipServiceMock.Setup(service => service.GetAllRelationshipsAsync()).ReturnsAsync(relationships);

        // Act
        await _familyTreeService.InitializeTreeAsync();

        // Assert
        Assert.True(_treeCache.Persons.ContainsKey(1));
        Assert.True(_treeCache.Persons.ContainsKey(2));
        Assert.True(_treeCache.Persons.ContainsKey(3));
        Assert.True(_treeCache.Persons.ContainsKey(4));
    }

    // GetImmediateRelatives Tests
    [Fact]
    public void GetImmediateRelatives_ShouldReturnRelatives()
    {
        // Arrange
        var person = new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true);
        var child = new PersonDTO(2, "Child", new DateTime(2010, 1, 1), true);
        person.Children.Add(child);
        child.Parents.Add(person);
        _treeCache.Persons[person.Id] = person;
        _treeCache.Persons[child.Id] = child;

        // Act
        var result = _familyTreeService.GetImmediateRelatives(1);

        // Assert
        Assert.Contains(child, result);
        Assert.Contains(person, _treeCache.Persons[child.Id].Parents);
    }

    [Fact]
    public void GetImmediateRelatives_WithMultipleRelatives_ShouldReturnAllRelatives()
    {
        // Arrange
        var person = new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true);
        var child1 = new PersonDTO(2, "Child1", new DateTime(2010, 1, 1), true);
        var child2 = new PersonDTO(3, "Child2", new DateTime(2015, 1, 1), false);
        var parent = new PersonDTO(4, "Parent", new DateTime(1950, 1, 1), true);
        person.Children.Add(child1);
        person.Children.Add(child2);
        person.Parents.Add(parent);
        child1.Parents.Add(person);
        child2.Parents.Add(person);
        _treeCache.Persons[person.Id] = person;
        _treeCache.Persons[child1.Id] = child1;
        _treeCache.Persons[child2.Id] = child2;
        _treeCache.Persons[parent.Id] = parent;

        // Act
        var result = _familyTreeService.GetImmediateRelatives(1);

        // Assert
        Assert.Contains(child1, result);
        Assert.Contains(child2, result);
        Assert.Contains(parent, result);
    }

    // CalculateAncestorAgeAtBirth Tests
    [Fact]
    public void CalculateAncestorAgeAtBirth_ShouldCalculateAge()
    {
        // Arrange
        var ancestor = new PersonDTO(1, "Ancestor", new DateTime(1950, 1, 1), true);
        var descendant = new PersonDTO(2, "Descendant", new DateTime(1980, 1, 1), true);
        ancestor.Children.Add(descendant);
        descendant.Parents.Add(ancestor);
        _treeCache.Persons[ancestor.Id] = ancestor;
        _treeCache.Persons[descendant.Id] = descendant;

        // Act
        var age = _familyTreeService.CalculateAncestorAgeAtBirth(1, 2);

        // Assert
        Assert.Equal(30, age);
    }

    [Fact]
    public void CalculateAncestorAgeAtBirth_WithDistantAncestor_ShouldCalculateAge()
    {
        // Arrange
        var ancestor = new PersonDTO(1, "Ancestor", new DateTime(1950, 1, 1), true);
        var parent = new PersonDTO(2, "Parent", new DateTime(1970, 1, 1), true);
        var descendant = new PersonDTO(3, "Descendant", new DateTime(2000, 1, 1), true);
        ancestor.Children.Add(parent);
        parent.Parents.Add(ancestor);
        parent.Children.Add(descendant);
        descendant.Parents.Add(parent);
        _treeCache.Persons[ancestor.Id] = ancestor;
        _treeCache.Persons[parent.Id] = parent;
        _treeCache.Persons[descendant.Id] = descendant;

        // Act
        var age = _familyTreeService.CalculateAncestorAgeAtBirth(1, 3);

        // Assert
        Assert.Equal(50, age);
    }

    // ResetTreeAsync Tests
    [Fact]
    public async Task ResetTreeAsync_ShouldClearTree()
    {
        // Act
        await _familyTreeService.ResetTreeAsync();

        // Assert
        _personServiceMock.Verify(service => service.ClearAllDbAsync(), Times.Once);
        _relationshipServiceMock.Verify(service => service.ClearAllDbAsync(), Times.Once);
    }

    [Fact]
    public async Task ResetTreeAsync_AfterMultipleOperations_ShouldClearTree()
    {
        // Arrange
        var person = new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true);
        _treeCache.Persons[person.Id] = person;

        // Act
        await _familyTreeService.ResetTreeAsync();

        // Assert
        _personServiceMock.Verify(service => service.ClearAllDbAsync(), Times.Once);
        _relationshipServiceMock.Verify(service => service.ClearAllDbAsync(), Times.Once);
        Assert.Empty(_treeCache.Persons);
    }

    // FindCommonAncestors Tests (continued)
    [Fact]
    public void FindCommonAncestors_ShouldReturnCommonAncestors()
    {
        // Arrange
        var ancestor = new PersonDTO(1, "Common Ancestor", new DateTime(1950, 1, 1), true);
        var person1 = new PersonDTO(2, "Person1", new DateTime(1980, 1, 1), true);
        var person2 = new PersonDTO(3, "Person2", new DateTime(1985, 1, 1), false);
        ancestor.Children.Add(person1);
        ancestor.Children.Add(person2);
        person1.Parents.Add(ancestor);
        person2.Parents.Add(ancestor);
        _treeCache.Persons[ancestor.Id] = ancestor;
        _treeCache.Persons[person1.Id] = person1;
        _treeCache.Persons[person2.Id] = person2;

        // Act
        var result = _familyTreeService.FindCommonAncestors(2, 3);

        // Assert
        Assert.Contains(ancestor, result);
    }

    [Fact]
    public void FindCommonAncestors_WithMultipleAncestors_ShouldReturnAllCommonAncestors()
    {
        // Arrange
        var commonAncestor1 = new PersonDTO(1, "Common Ancestor1", new DateTime(1940, 1, 1), true);
        var parent1 = new PersonDTO(3, "Parent1", new DateTime(1965, 1, 1), true);
        var parent2 = new PersonDTO(4, "Parent2", new DateTime(1970, 1, 1), false);
        var person1 = new PersonDTO(5, "Person1", new DateTime(1990, 1, 1), true);
        var person2 = new PersonDTO(6, "Person2", new DateTime(1995, 1, 1), false);

        commonAncestor1.Children.Add(parent1);
        commonAncestor1.Children.Add(parent2);
        parent1.Parents.Add(commonAncestor1);
        parent2.Parents.Add(commonAncestor1);
        parent1.Children.Add(person1);
        parent2.Children.Add(person2);
        person1.Parents.Add(parent1);
        person2.Parents.Add(parent2);

        _treeCache.Persons[commonAncestor1.Id] = commonAncestor1;
        _treeCache.Persons[parent1.Id] = parent1;
        _treeCache.Persons[parent2.Id] = parent2;
        _treeCache.Persons[person1.Id] = person1;
        _treeCache.Persons[person2.Id] = person2;

        // Act
        var result = _familyTreeService.FindCommonAncestors(5, 6);

        // Assert
        Assert.Contains(commonAncestor1, result);
    }
}