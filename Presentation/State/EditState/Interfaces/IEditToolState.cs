using FamilyTreeBlazor.presentation.State.Interfaces;

namespace FamilyTreeBlazor.presentation.State.EditState.Interfaces;

public interface IEditToolState : IAppState 
{
    void AddPerson(int id);
}

public enum EditStateEnum
{
    Initial,
    ChoosePerson,
    CreatePerson,
    CreateInitialPerson
}

public enum Relation
{
    Child,
    Parent,
    Spouse
}

public struct DisabledRelations
{
    public bool Parent;
    public bool Child;
    public bool Spouse;

    public DisabledRelations() { Parent = true; Child = true; Spouse = true; }
    public void Enable() { Parent = false; Child = false; Spouse = false; }
    public void Disable() { Parent = true; Child = true; Spouse = true; }
}
