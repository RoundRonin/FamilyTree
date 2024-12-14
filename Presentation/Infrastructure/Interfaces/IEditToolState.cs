namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public interface IEditToolState : IToolState
{
    int? EditId { get; set; }
    bool EditCreateNew { get; set; }
}

