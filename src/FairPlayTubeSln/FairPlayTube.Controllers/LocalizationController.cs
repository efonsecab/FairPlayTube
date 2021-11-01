
using AutoMapper;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// In charge of exposing localization values
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }

        private IMapper Mapper { get; }

        /// <summary>
        /// Initializes <see cref="LocalizationController"/>
        /// </summary>
        /// <param name="fairplaytubeDatabaseContext"></param>
        /// <param name="mapper"></param>
        public LocalizationController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            IMapper mapper)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.Mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ResourceModel[]> GetAllResources()
        {
            var currentCulture = CultureInfo.CurrentUICulture;
            var result = await this.FairplaytubeDatabaseContext.Resource
                .Include(p=>p.Culture)
                .Where(p=>p.Culture.Name == currentCulture.Name)
                .Select(p => this.Mapper.Map<Resource,ResourceModel>(p))
                .ToArrayAsync();
            if (result.Length == 0)
                result = await this.FairplaytubeDatabaseContext.Resource
                .Include(p => p.Culture)
                .Where(p => p.Culture.Name == "en-US")
                .Select(p => this.Mapper.Map<Resource, ResourceModel>(p))
                .ToArrayAsync();
            return result;
        }
    }
}
