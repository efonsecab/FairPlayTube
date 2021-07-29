using FairPlayTube.Client.Services;
using FairPlayTube.Models.Notifications;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomComponents.SignalR
{
    public partial class NotificationHub
    {

        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IAccessTokenProvider AccessTokenProvider { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private HubConnection HubConnection { get; set; }
        private List<NotificationModel> ReceivedNotifications { get; set; } = new List<NotificationModel>();
        private bool ShowNotifications { get; set; } = false;
        protected async override Task OnInitializedAsync()
        {
            var accessToken = await this.AccessTokenProvider.RequestAccessToken();
            if (accessToken.TryGetToken(out var token))
            {
                HubConnection = new HubConnectionBuilder()
                            .WithUrl(NavigationManager.ToAbsoluteUri(Common.Global.Constants.Hubs.NotificationHub),
                            options =>
                            {
                                options.AccessTokenProvider = () => Task.FromResult(token.Value);
                            })
                            .Build();

                HubConnection.On<NotificationModel>(Common.Global.Constants.Hubs.ReceiveMessage, async (model) =>
                {
                    ReceivedNotifications.Add(model);
                    await ToastifyService.DisplaySuccessNotification(model.Message);
                    StateHasChanged();
                });

                await HubConnection.StartAsync();
            }
        }

        private async Task Send(NotificationModel model)
        {
            await this.HubConnection.SendAsync(Common.Global.Constants.Hubs.SendMessage, model);
        }

        public bool IsConnected =>
        HubConnection.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            await HubConnection.DisposeAsync();
        }

        private async Task OnSendClick()
        {
            NotificationModel notificationModel = new NotificationModel()
            {
                Message = "TestMessage"
            };
            await this.Send(notificationModel);
        }

        private void OnShowNotificationClick()
        {
            this.ShowNotifications = true;
        }

        private void HideNotifications()
        {
            this.ShowNotifications = false;
        }
    }
}
