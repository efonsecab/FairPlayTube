using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Services
{
    public class ToastifyService
    {
        private IJSRuntime JSRuntime { get; }

        public ToastifyService(IJSRuntime jSRuntime)
        {
            this.JSRuntime = jSRuntime;
        }

        public async Task DisplaySuccessNotification(string text, int duration = 3000, bool newWindow = true,
            bool close = true, string gravity = "top", string position = "right",
            string backgroundColor = "linear-gradient(to right, #2193b0, #6dd5ed);",
            bool stopOnFocus = true)
        {
            await JSRuntime.InvokeVoidAsync("DisplaySuccessNotification", text, duration, newWindow,
                close, gravity, position, backgroundColor, stopOnFocus);
        }

        public async Task DisplayErrorNotification(string text, int duration = 0, bool newWindow = true,
            bool close = true, string gravity = "top", string position = "right",
            string backgroundColor = "#ff5959",
            bool stopOnFocus = true)
        {
            await JSRuntime.InvokeVoidAsync("DisplayErrorNotification", text, duration, newWindow,
                close, gravity, position, backgroundColor, stopOnFocus);
        }
    }
}
