using FamilyTreeBlazor.DAL.Infrastructure;

namespace FamilyTreeBlazor.DAL.Entities;

public class Person(int Id, string Name, DateTime BirthDateTime, bool Sex) : IEntity
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public DateTime BirthDateTime { get; set; } = BirthDateTime;
    public bool Sex { get; set; } = Sex;
}
