using FairPlayTube.Common.Localization;
using FairPlayTube.Models.UserMessage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
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
        [Inject]
        private IStringLocalizer<MessageSender> Localizer { get; set; }
        private UserMessageModel UserMessageModel { get; set; } = new();

        protected override void OnParametersSet()
        {
            this.UserMessageModel.ToApplicationUserId = this.ToApplicationUserId;
        }

        private async Task OnValidSubmit()
        {
            await OnSendAction.InvokeAsync(this.UserMessageModel);
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Name")]
        public const string NameTextKey = "NameText";
        [ResourceKey(defaultValue: "Message")]
        public const string MessageTextKey = "MessageText";
        [ResourceKey(defaultValue: "Cancel")]
        public const string CancelTextKey = "CancelText";
        [ResourceKey(defaultValue:"Send")]
        public const string SendTextKey = "SendText";
        #endregion Resource Keys
    }
}