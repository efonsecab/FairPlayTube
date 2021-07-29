using FairPlayTube.Common.Interfaces;
using FairPlayTube.Common.Providers;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.DataAccess.Data
{
    public partial class FairplaytubeDatabaseContext
    {
        private ICurrentUserProvider CurrentUserProvider { get; }

        public FairplaytubeDatabaseContext(DbContextOptions<FairplaytubeDatabaseContext> options,
            ICurrentUserProvider currentUserProvider) : base(options)
        {
            this.CurrentUserProvider = currentUserProvider;
        }


        public override int SaveChanges()
        {
            ValidateAndSetDefaults();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ValidateAndSetDefaults();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ValidateAndSetDefaults();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ValidateAndSetDefaults();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ValidateAndSetDefaults()
        {
            //Check https://www.bricelam.net/2016/12/13/validation-in-efcore.html
            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            string ipAddresses = String.Empty;
            string assemblyFullName = String.Empty;
            string rowCretionUser = String.Empty;
            if (entities.Any(p => p is IOriginatorInfo))
            {
                ipAddresses = String.Join(",", IpAddressProvider.GetCurrentHostIPv4Addresses());
                assemblyFullName = System.Reflection.Assembly.GetEntryAssembly().FullName;
                rowCretionUser = this.CurrentUserProvider.GetUsername();
            }
            foreach (var entity in entities)
            {
                if (entity is IOriginatorInfo)
                {
                    IOriginatorInfo entityWithOriginator = entity as IOriginatorInfo;
                    if (String.IsNullOrWhiteSpace(entityWithOriginator.SourceApplication))
                    {
                        entityWithOriginator.SourceApplication = assemblyFullName;
                    }
                    if (String.IsNullOrWhiteSpace(entityWithOriginator.OriginatorIpaddress))
                    {
                        entityWithOriginator.OriginatorIpaddress = ipAddresses;
                    }
                    if (entityWithOriginator.RowCreationDateTime == DateTimeOffset.MinValue)
                    {
                        entityWithOriginator.RowCreationDateTime = DateTimeOffset.UtcNow;
                    }
                    if (String.IsNullOrWhiteSpace(entityWithOriginator.RowCreationUser))
                    {
                        entityWithOriginator.RowCreationUser = rowCretionUser;
                    }
                }
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(
                    entity,
                    validationContext,
                    validateAllProperties: true);
            }
        }
    }
}
