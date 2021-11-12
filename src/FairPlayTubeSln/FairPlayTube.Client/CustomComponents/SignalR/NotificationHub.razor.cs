using FairPlayTube.Client.Services;
using FairPlayTube.Common.Global;
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

                HubConnection.On<NotificationModel>(Common.Global.Constants.Hubs.ReceiveMessage, (model) =>
                {
                    ReceivedNotifications.Add(model);
                    ToastifyService.DisplaySuccessNotification(model.Message);
                    StateHasChanged();
                });

                await HubConnection.StartAsync();
            }
        }

        public bool IsConnected =>
        HubConnection.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            await HubConnection.DisposeAsync();
        }

        private void OnShowNotificationClick()
        {
            this.ShowNotifications = true;
        }

        private void HideNotifications()
        {
            this.ShowNotifications = false;
        }

        private static string GetVideoDetailsUrl(string videoId)
        {
            return Constants.PublicVideosPages.Details.Replace("{VideoId}", videoId);
        }
    }
}
