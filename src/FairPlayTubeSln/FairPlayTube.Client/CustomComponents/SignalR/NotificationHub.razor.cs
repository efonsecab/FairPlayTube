using FairPlayTube.Models.Notifications;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomComponents.SignalR
{
    public partial class NotificationHub
    {

        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private HubConnection HubConnection { get; set; }
        private List<NotificationModel> ReceivedNotifications { get; set; } = new List<NotificationModel>();
        private bool ShowNotifications { get; set; } = false;
        protected async override Task OnInitializedAsync()
        {
            HubConnection = new HubConnectionBuilder()
                        .WithUrl(NavigationManager.ToAbsoluteUri(Common.Global.Constants.Hubs.NotificationHub))
                        .Build();

            HubConnection.On<NotificationModel>(Common.Global.Constants.Hubs.ReceiveMessage, (model) =>
            {
                ReceivedNotifications.Add(model);
                StateHasChanged();
            });

            await HubConnection.StartAsync();
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
