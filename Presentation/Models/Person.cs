namespace FamilyTreeBlazor.presentation.Models;

public class Person(int Id, string Name, DateTime BirthDateTime, bool Sex)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public DateTime BirthDateTime { get; set; } = BirthDateTime;
    public bool Sex { get; set; } = Sex;


    public int TreeDepth { get; set; } = 0;
}

