namespace FamilyTreeBlazor.BLL.DTOs;

internal class PersonDTO(int Id, string Name, DateTime BirthDateTime, bool Sex)
{
    public int Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public DateTime BirthDateTime { get; set; } = BirthDateTime;
    public bool Sex { get; set; } = Sex;
    public List<PersonDTO> Parents { get; set; } = [];
    public List<PersonDTO> Children { get; set; } = [];
    public PersonDTO? Spouce { get; set; }
}
