using FamilyTreeBlazor.DAL.Infrastructure;

namespace FamilyTreeBlazor.presentation.Entities;

public enum CardState
{
    Default,
    HighlightedChosen,
    ChooseSpouse,
    ChooseChild,
    ChooseParent,
    HighlightedCommonAncestor,
}

public class Person(int Id, string Name, DateTime BirthDateTime, bool Sex) : IEntity
{
    public int Id { get; } = Id;
    public string Name { get; set; } = Name;
    public DateTime BirthDateTime { get; set; } = BirthDateTime;
    public bool Sex { get; set; } = Sex;

    
    public CardState State { get; set; } = CardState.Default;
    public int TreeDepth { get; set; } = 0;
}

