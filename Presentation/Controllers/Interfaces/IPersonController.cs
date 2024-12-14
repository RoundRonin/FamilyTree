using FamilyTreeBlazor.presentation.Entities;

namespace FamilyTreeBlazor.presentation.Controllers.Interfaces;

public interface IPersonController
{
    public void CreateRelation(int targetId);
    public void AddPerson(string name, DateTime dateTime, bool sex);
    public int? GetAncestorAge(int Id1, int Id2);
    public Person GetPerson(int id);
}