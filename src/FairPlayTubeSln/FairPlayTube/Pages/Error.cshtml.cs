using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FairPlayTube.Pages
{
    /// <summary>
    /// Error Model
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// Request Id
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// Show Request Id
        /// </summary>

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// OnGet
        /// </summary>
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
