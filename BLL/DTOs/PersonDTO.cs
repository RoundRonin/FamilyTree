namespace FamilyTreeBlazor.BLL.DTOs;

public class PersonDTO(string Name, DateTime BirthDateTime, bool Sex, int? Id = null)
{
    public int? Id { get; set; } = Id; // Nullable Id to handle creation of new instances
    public string Name { get; set; } = Name;
    public DateTime BirthDateTime { get; set; } = BirthDateTime;
    public bool Sex { get; set; } = Sex;
    public List<PersonDTO> Parents { get; set; } = [];
    public List<PersonDTO> Children { get; set; } = [];
    public PersonDTO? Spouse { get; set; }
}
