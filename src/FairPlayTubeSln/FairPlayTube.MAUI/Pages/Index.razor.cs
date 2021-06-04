using System;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace FairPlayTube.MAUI.Pages
{
    public partial class Index
    {
        [Inject]
        private System.Net.Http.HttpClient http { get; set; }
        private string Result { get; set; }

        protected async override Task OnInitializedAsync()
        {
            try
            {
                this.Result = await http.GetStringAsync("https://petstore.swagger.io/v2/store/inventory");
            }
            catch (Exception ex)
            {
                this.Result = ex.ToString();
            }
        }
    }
}
