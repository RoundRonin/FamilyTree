using FamilyTreeBlazor.presentation.Entities;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure.Interfaces;

public interface IToolState
{
    RenderFragment RenderPanel();
    RenderFragment RenderCard(Person person);
    void HandleId(int Id);
    object GetSpecificState();
}

