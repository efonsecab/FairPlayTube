using FairPlayTube.ClientServices;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FairPlayTube.MauiBlazor.Shared
{
    public partial class Error
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Inject]
        private ClientSideErrorLogClientService ClientSideErrorLogClientService { get; set; }
        private static List<Exception> ExceptionsList = new();

        public async Task ProcessErrorAsync(Exception ex)
        {
            ExceptionsList.Add(ex);
            Logger.LogError("Error:ProcessError - Type: {Type} Message: {Message}",
                ex.GetType(), ex.Message);
            await this.ClientSideErrorLogClientService.AddClientSideErrorAsync(
                new Models.ClientSideErrorLog.CreateClientSideErrorLogModel()
                {
                    FullException = ex.ToString(),
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                }
            );
        }

        public List<Exception> GetExceptionsList() => ExceptionsList;
    }
}
