using FamilyTreeBlazor.presentation.Services.Interfaces;
using FamilyTreeBlazor.presentation.Controllers.Interfaces;
using Microsoft.JSInterop;
using FamilyTreeBlazor.presentation.Models;
using FamilyTreeBlazor.presentation.State.Interfaces;
using FamilyTreeBlazor.presentation.Services.Commands;

namespace FamilyTreeBlazor.presentation.Controllers;

public class TreeController : IDisposable, ITreeController
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IAppStateContext _appStateContext;
    private bool _disposed = false;

    public IPresentationService PresentationService { get; private set; }

    public TreeController(IJSRuntime jsRuntime, IAppStateContext appStateContext, IPresentationService presentationService)
    {
        _jsRuntime = jsRuntime;
        _appStateContext = appStateContext;
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

            _appStateContext.OnChange += async () => await OnAppStateChanged();
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
        var command = new HandleIdCommand(int.Parse(personId));
        _appStateContext.CurrentToolState.Fire(command);
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
            _appStateContext.OnChange -= async () => await OnAppStateChanged();
            PresentationService.OnDataChanged -= async () => await UpdateTree();
        }

        _disposed = true;
    }

    private async Task OnAppStateChanged()
    {
        await _jsRuntime.InvokeVoidAsync("setDraggingOn", _appStateContext.DraggingOn);

        var command = new SetEditStateJSCommand(_jsRuntime);
        _appStateContext.CurrentToolState.Fire(command);
    }
}