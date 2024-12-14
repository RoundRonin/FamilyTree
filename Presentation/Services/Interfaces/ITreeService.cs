using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
namespace FamilyTreeBlazor.presentation.Services.Interfaces;

public interface ITreeService
{
    public event Action OnDataChanged;

    // Getting general data
    public ITreeCache CachedTree { get; }
    public List<Person> PersonList { get; }
    public List<Relationship> RelationshipList { get; }
    public Person GetPerson(int id);

    // General tree workflow
    public void UpdateTree();
    public void DeleteTree();

    // Geting relevant business info
    public IEnumerable<Person> GetChildren(int Id);
    public IEnumerable<Person> GetParents(int Id);
    public Person? GetSpouse(int Id);
    public int? GetAncestorAge(int Id1, int Id2);
    public Dictionary<int, CardState> GetCommonAncestors(int Id1, int Id2);
    public Dictionary<int, CardState> GetPersonAncestors(int Id);

    // Adding new items
    public void AddPersonRelationship(Person person, Relationship rel, Relation type);
    public void AddRelationship(Relationship rel, Relation newRelation);
}
