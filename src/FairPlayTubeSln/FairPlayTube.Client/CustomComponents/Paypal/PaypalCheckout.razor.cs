using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.Paypal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomComponents.Paypal
{
    public partial class PaypalCheckout
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private UserProfileClientService UserProfileClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Parameter]
        public EventCallback OnFundsAdded { get; set; }
        private DotNetObjectReference<PaypalCheckout> objRef;

        protected override void OnInitialized()
        {
            this.objRef = DotNetObjectReference.Create(this);
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initPayPalButton", objRef);
                StateHasChanged();
            }
        }

        [JSInvokable]
        public async void OnApprove(PaypalCheckoutApprovedDataModel data, PaypalCheckoutApprovedDetailsModel details)
        {
            try
            {
                await this.UserProfileClientService.AddFunds(data.orderID);
                await ToastifyService.DisplaySuccessNotification($"Fundas have been added to your {Common.Global.Constants.Titles.AppTitle} Wallter");
                await OnFundsAdded.InvokeAsync();
            }
            catch (Exception ex)
            {
                await ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }
    }
}
