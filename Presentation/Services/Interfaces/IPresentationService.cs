using FamilyTreeBlazor.BLL.Infrastructure;
using FamilyTreeBlazor.presentation.Models;

namespace FamilyTreeBlazor.presentation.Services.Interfaces;

public interface IPresentationService
{
    public event Action OnDataChanged;

    // Getting general data
    public ITreeCache CachedTree { get; }
    public List<Person> PersonList { get; }
    public List<Relationship> RelationshipList { get; }
    public Dictionary<int, Person> PersonDict { get; }

    public void NotifyOnChanged();

    // General tree workflow (unrelated to business)
    public void UpdateTree();
    public void DeleteTree();

    // Essential presentation fucntions
    public void AddInitialPerson(Person person);
}
