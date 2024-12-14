using FamilyTreeBlazor.presentation.Infrastructure;
using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.Controllers.Interfaces;
using Microsoft.JSInterop;
using FamilyTreeBlazor.presentation.Infrastructure.Interfaces;
using FamilyTreeBlazor.presentation.Models;

namespace FamilyTreeBlazor.presentation.Controllers;

public class TreeController : IDisposable, ITreeController
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IAppStateService _appStateService;
    private bool _disposed = false;

    public IPresentationService PresentationService { get; private set; }

    public TreeController(IJSRuntime jsRuntime, IAppStateService appStateService, IPresentationService presentationService)
    {
        _jsRuntime = jsRuntime;
        _appStateService = appStateService;
        PresentationService = presentationService;

        Initialize();
    }

    private void Initialize()
    {
        PresentationService.OnDataChanged += async () => await UpdateTree();
    }

    public async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Console.WriteLine("OnAfterRenderAsync firstRender");

            await _jsRuntime.InvokeVoidAsync("registerBlazorClickHandler", DotNetObjectReference.Create(this));
            Console.WriteLine("Blazor click handler registered");

            await RenderTree();

            _appStateService.OnChange += async () =>
            {
                await _jsRuntime.InvokeVoidAsync("setDraggingOn", _appStateService.DraggingOn);
                bool isEditMode = _appStateService.CurrentToolState is EditToolState;
                bool isChoosingMode = false;
                if (isEditMode)
                {
                    isChoosingMode = _appStateService.GetSpecificState<EditToolState>().State == EditState.ChoosePerson;
                }
                await _jsRuntime.InvokeVoidAsync("setEditMode", isEditMode && isChoosingMode);
            };
        }
    }

    public async Task RenderTree()
    {
        var (nodesHtml, links) = GetTreeData();

        await _jsRuntime.InvokeVoidAsync("renderD3Graph", nodesHtml, links, 180);
        await _jsRuntime.InvokeVoidAsync("insertComponents", "person-card");
    }

    private async Task UpdateTree()
    {
        var (nodesHtml, links) = GetTreeData();

        await _jsRuntime.InvokeVoidAsync("updateGraph", nodesHtml, links);
        //await _jsRuntime.InvokeVoidAsync("insertComponents", "person-card");
    }

    private (List<PersonNode> nodesHtml, List<RelationLink> links) GetTreeData()
    {
        var nodesHtml = PresentationService.PersonList.Select(node => new
        PersonNode{
            Id = node.Id,
            Name = node.Name,
            TreeDepth = node.TreeDepth,
            Person = node,
            HtmlId = $"person-card-container-{node.Id}"
        }).ToList();

        var links = PresentationService.RelationshipList.Select(link => new
        RelationLink {
            Source = link.PersonId1,
            Target = link.PersonId2,
            RelationshipType = link.RelationshipType
        }).ToList();

        return (nodesHtml, links);
    }

    [JSInvokable]
    public void BlazorClickHandler(string personId)
    {
        Console.WriteLine($"BlazorClickHandler invoked with Person ID: {personId}");
        _appStateService.CurrentToolState.HandleId(int.Parse(personId));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _appStateService.OnChange -= async () => await OnAppStateChanged();
            PresentationService.OnDataChanged -= async () => await UpdateTree();
        }

        _disposed = true;
    }

    private async Task OnAppStateChanged()
    {
        await _jsRuntime.InvokeVoidAsync("setDraggingOn", _appStateService.DraggingOn);
        bool isEditMode = _appStateService.CurrentToolState is EditToolState;
        bool isChoosingMode = false;
        if (isEditMode)
        {
            isChoosingMode = _appStateService.GetSpecificState<EditToolState>().State == EditState.ChoosePerson;
        }
        await _jsRuntime.InvokeVoidAsync("setEditMode", isEditMode && isChoosingMode);
    }
}