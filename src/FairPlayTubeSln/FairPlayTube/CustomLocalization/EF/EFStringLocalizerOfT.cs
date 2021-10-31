using FairPlayTube.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.CustomLocalization.EF
{
    /// <summary>
    /// Handles EF-based localization
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EFStringLocalizer<T> : IStringLocalizer<T>
    {
        private readonly FairplaytubeDatabaseContext _db;

        /// <summary>
        /// Initializes <see cref="EFStringLocalizer{T}"/>
        /// </summary>
        /// <param name="db"></param>
        public EFStringLocalizer(FairplaytubeDatabaseContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves the value for the given key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        /// <summary>
        /// Tetrieves the value for the given key using the specified arguments
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        /// <summary>
        /// Sets the Culture to use
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new EFStringLocalizer(_db);
        }

        /// <summary>
        /// Gets all of the values
        /// </summary>
        /// <param name="includeAncestorCultures"></param>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            string typeFullName = typeof(T).FullName;
            return _db.Resource
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name
                && r.Type == typeFullName
                )
                .Select(r => new LocalizedString(r.Key, r.Value, true));
        }

        private string GetString(string name)
        {
            string typeFullName = typeof(T).FullName;
            return _db.Resource
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name &&
                r.Type == typeFullName
                )
                .FirstOrDefault(r => r.Key == name)?.Value;
        }
    }
}
