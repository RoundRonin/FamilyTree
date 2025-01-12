namespace FamilyTreeBlazor.presentation.Models;

public class PersonNode
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TreeDepth { get; set; }
    public string HtmlId { get; set; }
    public Person Person { get; set; }
}

