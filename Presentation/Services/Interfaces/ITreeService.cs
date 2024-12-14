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
    public void InitializeTree();
    public void UpdateTree();
    public void DeleteTree();

    // Geting relevant business info
    public Person FindPerson(int Id);
    public PersonListDTO GetKids(int Id);
    public PersonListDTO GetParents(int Id);
    public Person? GetSpouse(int Id);
    public Dictionary<int, CardState> GetCommonAncestors(int Id1, int Id2);

    // Adding new items
    public void AddPersonRelationship(Person person, Relationship rel, Relation type);
    public void AddRelationship(Relationship rel);
}
