using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Infrastructure;
using FamilyTreeBlazor.presentation.Entities;

namespace FamilyTreeBlazor.presentation.Services;

class TreeService(ITreeCache treeCache) : ITreeService 
{
    public ITreeCache CachedTree { get; private set; } = treeCache;
    public List<Person> PersonList { get; private set; } = [];
    public List<Relationship> RelationshipList { get; private set; } = [];

    public void InitializeTree()
    {
        throw new NotImplementedException();
    }

    public void UpdateTree()
    {
        throw new NotImplementedException();
    }

    public void DeleteTree() { 
        throw new NotImplementedException(); 
    }

    public void ChangeStateById(List<int> ids, CardState cardState)
    {
        throw new NotImplementedException();
    }

    public void AddPersonRelationship(Person person, Relationship relationship)
    {
        throw new NotImplementedException();
    }

    public void AddRelationship(Relationship relationship)
    {
        throw new NotImplementedException();
    }
}