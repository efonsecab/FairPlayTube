using FairPlayTube.Models.UserMessage;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace FairPlayTube.Components.Notifications
{
    public partial class MessageSender
    {
        [Parameter]
        public long ToApplicationUserId { get; set; }
        [Parameter]
        public string ToApplicationUserFullName { get; set; }
        [Parameter]
        public EventCallback<UserMessageModel> OnSendAction { get; set; }
        [Parameter]
        public EventCallback OnCancelAction { get; set; }

        private UserMessageModel UserMessageModel { get; set; } = new();

        protected override void OnParametersSet()
        {
            this.UserMessageModel.ToApplicationUserId = this.ToApplicationUserId;
        }

        private async Task OnValidSubmit()
        {
            await OnSendAction.InvokeAsync(this.UserMessageModel);
        }
    }
}