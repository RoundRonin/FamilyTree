using FamilyTreeBlazor.presentation.Models;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.State.Interfaces;

public interface IState 
{
    void EnterState();
    void ExitState();
    RenderFragment RenderPanel();
    RenderFragment RenderCard(Person person);
}
