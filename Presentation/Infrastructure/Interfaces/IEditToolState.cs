namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public enum EditState
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

public interface IEditToolState : IToolState
{
    public EditState State { get; set; }
}

