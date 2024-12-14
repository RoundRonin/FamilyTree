using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public interface IToolState
{
    RenderFragment RenderPanel();
    RenderFragment RenderCard(string name, DateTime birthDay);
    object GetSpecificState();
}

