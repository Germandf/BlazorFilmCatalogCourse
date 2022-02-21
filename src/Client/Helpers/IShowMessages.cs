using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Client.Helpers
{
    public interface IShowMessages
    {
        Task ShowErrorMessage(string message);
        Task ShowSuccessMessage(string message);
    }

    public class ShowMessages : IShowMessages
    {
        private IJSRuntime _js;

        public ShowMessages(IJSRuntime js)
        {
            _js = js;
        }

        public async Task ShowErrorMessage(string message)
        {
            await ShowMessage("¡Error!", message, "error");
        }

        public async Task ShowSuccessMessage(string message)
        {
            await ShowMessage("¡Éxito!", message, "success");
        }

        private async ValueTask ShowMessage(string title, string message, string messageType)
        {
            await _js.InvokeVoidAsync("Swal.fire", title, message, messageType);
        }
    }
}
