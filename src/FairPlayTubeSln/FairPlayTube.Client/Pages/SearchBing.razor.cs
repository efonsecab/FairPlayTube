using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Global;
using FairPlayTube.Models.BingSearch;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages
{
    [Route(Constants.RootPagesRoutes.SearchBing)]
    public partial class SearchBing
    {
        [Parameter]
        public string SearchTerm { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private SearchClientService SearchClientService { get; set; }
        private bool IsLoading { get; set; }
        private BingSearchVideoModel[] AllResults { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllResults = await this.SearchClientService.SearchBingVideosAsync(SearchTerm);
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
