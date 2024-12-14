using FamilyTreeBlazor.DAL.Infrastructure;

namespace FamilyTreeBlazor.DAL.Entities;

public class Person(string Name, DateTime BirthDateTime, bool Sex) : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = Name;
    public DateTime BirthDateTime { get; set; } = BirthDateTime.ToUniversalTime();
    public bool Sex { get; set; } = Sex;
}
