using Blazored.Toast.Services;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Services
{
    public class ToastifyService
    {
        private IToastService ToastService { get; }

        public ToastifyService(IToastService toastService)
        {
            this.ToastService = toastService;
        }

        public void DisplaySuccessNotification(string message, string title=null)
        {
            ToastService.ShowSuccess(message, title);
        }

        public void DisplayErrorNotification(string message, string title=null)
        {
            ToastService.ShowError(message, title);
        }

        public void DisplayInformationNotification(string message, string title=null)
        {
            ToastService.ShowInfo(message, title);
        }

        public void DisplayWarningNotification(string message, string title=null)
        {
            ToastService.ShowWarning(message, title);
        }
    }
}
