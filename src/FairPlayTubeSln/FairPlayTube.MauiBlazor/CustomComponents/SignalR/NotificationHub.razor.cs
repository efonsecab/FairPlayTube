using Blazored.Toast.Services;
using FairPlayTube.Common.Global;
using FairPlayTube.MauiBlazor.Features.LogOn;
using FairPlayTube.Models.Notifications;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace FairPlayTube.MauiBlazor.CustomComponents.SignalR
{
    public partial class NotificationHub
    {

        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IToastService ToastService { get; set; }
        private HubConnection HubConnection { get; set; }
        private List<NotificationModel> ReceivedNotifications { get; set; } = new List<NotificationModel>();
        private bool ShowNotifications { get; set; } = false;
        protected async override Task OnInitializedAsync()
        {
            var token = UserState.UserContext.AccessToken;
            HubConnection = new HubConnectionBuilder()
                        .WithUrl(NavigationManager.ToAbsoluteUri(Common.Global.Constants.Hubs.NotificationHub),
                        options =>
                        {
                            options.AccessTokenProvider = () => Task.FromResult(token);
                        })
                        .Build();

            HubConnection.On<NotificationModel>(Common.Global.Constants.Hubs.ReceiveMessage, (model) =>
            {
                ReceivedNotifications.Add(model);
                ToastService.ShowInfo(model.Message);
                StateHasChanged();
            });

            await HubConnection.StartAsync();
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
