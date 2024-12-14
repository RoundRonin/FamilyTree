using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Entities;
namespace FamilyTreeBlazor.presentation.Services.Interfaces;

public interface ITreeService
{
    public ITreeCache CachedTree { get; }
    public List<Person> PersonList { get; }
    public List<Relationship> RelationshipList { get; }

    public void InitializeTree();
    public void UpdateTree();
    public void DeleteTree();

    public void ChangeStateById(List<int> ids, CardState cardState);
    public void AddPersonRelationship(Person person, Relationship relationship);
    public void AddRelationship(Relationship relationship);
}
