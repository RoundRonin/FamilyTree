using Microsoft.JSInterop;

namespace FamilyTreeBlazor.presentation.Controllers.Interfaces
{
    public interface ITreeController : IDisposable
    {
        Task OnAfterRenderAsync(bool firstRender);
        Task RenderTree();
        [JSInvokable]
        void BlazorClickHandler(string personId);
    }
}
