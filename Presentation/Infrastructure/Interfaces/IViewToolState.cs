namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public enum ViewState
{
    Initial,
    View
}

public interface IViewToolState : IToolState
{
    public ViewState State { get; set; }
}

