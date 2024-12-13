using FamilyTreeBlazor.BLL;
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
}
