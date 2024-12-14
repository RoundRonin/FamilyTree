using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Entities;
namespace FamilyTreeBlazor.presentation.Services.Interfaces;

public enum InsertionType
{
    child,
    parent,
    spouse
}

public interface ITreeService
{
    // Getting general data
    public ITreeCache CachedTree { get; }
    public List<Person> PersonList { get; }
    public List<Relationship> RelationshipList { get; }
    public Person GetPerson(int id);

    // General tree workflow
    public void InitializeTree();
    public void UpdateTree();
    public void DeleteTree();

    // Geting relevant business info
    public Person FindPerson(int Id);
    public PersonListDTO GetKids(int Id);
    public PersonListDTO GetParents(int Id);
    public Person GetSpouse(int Id);
    public Dictionary<int, CardState> GetCommonAncestors(int Id1, int Id2);

    // Adding new items
    public void AddPersonRelationship(Person person, Relationship rel, InsertionType type);
    public void AddRelationship(Relationship rel);
}
