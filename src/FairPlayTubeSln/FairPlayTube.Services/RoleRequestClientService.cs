using FairPlayTube.DataAccess.Data;
using FairPlayTube.Models.RoleRequest;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class RoleRequestClientService
    {
        private readonly FairplaytubeDatabaseContext FairplaytubeDatabaseContext;
        public RoleRequestClientService(FairplaytubeDatabaseContext FairplaytubeDatabaseContext)
        {
            this.FairplaytubeDatabaseContext = FairplaytubeDatabaseContext;
        }

        public Task AddRoleRequestAsync(AddRoleRequestModel addRoleRequestModel, CancellationToken cancellationToken)
        {
            //TODO: IT IS REQUIRED THE DB CONTEXT FOR ROLE REQUEST TABLE
            return Task.CompletedTask;
        }
    }
}
