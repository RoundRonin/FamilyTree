namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public enum EditState
{
    Initial,
    ChoosePerson,
    CreatePerson
}

public interface IEditToolState : IToolState
{
    public EditState State { get; set; }
}

