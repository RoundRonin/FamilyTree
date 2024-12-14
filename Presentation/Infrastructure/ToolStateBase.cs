using FamilyTreeBlazor.presentation.Entities;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FamilyTreeBlazor.presentation.Infrastructure;

public abstract class ToolStateBase(IStateNotifier stateNotifier) : IToolState
{
    protected readonly IStateNotifier _stateNotifier = stateNotifier;

    public abstract RenderFragment RenderPanel();
    public abstract RenderFragment RenderCard(Person person);
    public abstract void HandleId(int Id);
    public abstract object GetSpecificState();

    protected void NotifyStateChanged()
    {
        _stateNotifier.NotifyStateChanged();
    }
}