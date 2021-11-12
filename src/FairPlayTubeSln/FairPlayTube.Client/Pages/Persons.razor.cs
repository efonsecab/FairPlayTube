using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Persons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages
{
    [Route(Common.Global.Constants.RootPagesRoutes.Persons)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class Persons
    {
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }

        [Inject]
        private IStringLocalizer<Persons> Localizer { get; set; }
        private PersonModel[] AllPersons { get; set; }
        private bool IsLoading { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllPersons = await this.VideoClientService.GetPersonsAsync();
            }
            catch (Exception ex)
            {
                ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"Persons In Videos")]
        public const string PersonsInVideosTitleKey = "PersonsInVideosTitle";
        #endregion Resource Keys
    }
}
