using FairPlayTube.DataAccess.Data;
using FairPlayTube.Models.ClientSideErrorLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class ClientSideErrorLogService
    {
        private readonly FairplaytubeDatabaseContext FairplaytubeDatabaseContext;

        public ClientSideErrorLogService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
        }

        public async Task AddClientSideErrorAsync(CreateClientSideErrorLogModel createClientSideErrorLogModel,
            CancellationToken cancellationToken)
        {
            await this.FairplaytubeDatabaseContext.ClientSideErrorLog.AddAsync(
                new DataAccess.Models.ClientSideErrorLog() 
                {
                    FullException=createClientSideErrorLogModel.FullException,
                    Message=createClientSideErrorLogModel.Message,
                    StackTrace=createClientSideErrorLogModel.StackTrace
                }, cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
        }

    }
}
