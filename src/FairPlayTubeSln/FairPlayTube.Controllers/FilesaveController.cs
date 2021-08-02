using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Models.FileUpload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Services;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// In charge to saving files
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesaveController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<FilesaveController> logger;

        private AzureBlobStorageService AzureBlobStorageService { get; set; }
        private DataStorageConfiguration DataStorageConfiguration { get; set; }
        private ICurrentUserProvider CurrentUserProvider { get; set; }

        /// <summary>
        /// Initializes <see cref="FilesaveController"/>
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="azureBlobStorageService"></param>
        /// <param name="dataStorageConfiguration"></param>
        /// <param name="currentUserProvider"></param>
        public FilesaveController(IWebHostEnvironment env,
            ILogger<FilesaveController> logger,
            AzureBlobStorageService azureBlobStorageService,
            DataStorageConfiguration dataStorageConfiguration,
            ICurrentUserProvider currentUserProvider)
        {
            this.env = env;
            this.logger = logger;
            this.AzureBlobStorageService = azureBlobStorageService;
            this.DataStorageConfiguration = dataStorageConfiguration;
            this.CurrentUserProvider = currentUserProvider;
        }

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="files"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        [DisableRequestSizeLimit]
        //[RequestSizeLimit(1073741824)] //1GB
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult<IList<UploadResult>>> PostFile(
            [FromForm] IEnumerable<IFormFile> files, CancellationToken cancellationToken)
        {
            //TODO: Disabling request limits it is not recommended. We'll use it as a temporary measure, but needs to be changed
            var userAzueAdB2cObjectId = this.CurrentUserProvider.GetObjectId();
            var maxAllowedFiles = 3;
            long maxFileSize = Common.Global.Constants.UploadLimits.MaxBytesAllowed;
            var filesProcessed = 0;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            List<UploadResult> uploadResults = new();

            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay =
                    WebUtility.HtmlEncode(untrustedFileName);

                if (filesProcessed < maxAllowedFiles)
                {
                    if (file.Length == 0)
                    {
                        logger.LogInformation("{FileName} length is 0 (Err: 1)",
                            trustedFileNameForDisplay);
                        uploadResult.ErrorCode = 1;
                    }
                    else if (file.Length > maxFileSize)
                    {
                        logger.LogInformation("{FileName} of {Length} bytes is " +
                            "larger than the limit of {Limit} bytes (Err: 2)",
                            trustedFileNameForDisplay, file.Length, maxFileSize);
                        uploadResult.ErrorCode = 2;
                    }
                    else
                    {
                        try
                        {
                            var fileExtension = Path.GetExtension(file.FileName);
                            trustedFileNameForFileStorage = $"{Path.GetRandomFileName()}{fileExtension}";
                            string fileRelativePath = $"User/{userAzueAdB2cObjectId}/{trustedFileNameForFileStorage}";

                            var blobUploadResult = await this.AzureBlobStorageService.UploadFileAsync(
                                this.DataStorageConfiguration.UntrustedUploadsContainerName, fileRelativePath,
                                file.OpenReadStream(), true, cancellationToken);

                            logger.LogInformation("{FileName} saved at {Path}",
                                trustedFileNameForDisplay, fileRelativePath);
                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = trustedFileNameForFileStorage;
                        }
                        catch (IOException ex)
                        {
                            logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                                trustedFileNameForDisplay, ex.Message);
                            uploadResult.ErrorCode = 3;
                            throw;
                        }
                    }

                    filesProcessed++;
                }
                else
                {
                    logger.LogInformation("{FileName} not uploaded because the " +
                        "request exceeded the allowed {Count} of files (Err: 4)",
                        trustedFileNameForDisplay, maxAllowedFiles);
                    uploadResult.ErrorCode = 4;
                }

                uploadResults.Add(uploadResult);
            }

            return new CreatedResult(resourcePath, uploadResults);
        }
    }
}
