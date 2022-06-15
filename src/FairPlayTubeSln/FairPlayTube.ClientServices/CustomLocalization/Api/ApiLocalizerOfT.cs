using FairPlayTube.Common.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.ClientServices.CustomLocalization.Api
{
    public class ApiLocalizer<T> : IStringLocalizer<T>
    {
        private readonly LocalizationClientService _localizationClientService;

        public ApiLocalizer(LocalizationClientService localizationClientService)
        {
            _localizationClientService = localizationClientService;
        }
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new ApiLocalizer(_localizationClientService);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            return _localizationClientService.GetAllStrings();
        }

        private string GetString(string name)
        {
            string typeName = typeof(T).FullName;
            var result = _localizationClientService.GetString(typeName, name);
            if (String.IsNullOrWhiteSpace(result))
            {
                var singleTypeToCheck = typeof(T);
                string typeFullName = singleTypeToCheck.FullName;
                var fields = singleTypeToCheck.GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy
                    );
                foreach (var singleField in fields)
                {
                    var resourceKeyAttributes =
                        singleField.GetCustomAttributes<ResourceKeyAttribute>();
                    if (resourceKeyAttributes != null && resourceKeyAttributes.Any())
                    {
                        ResourceKeyAttribute keyAttribute = resourceKeyAttributes.Single();
                        var defaultValue = keyAttribute.DefaultValue;
                        string key = singleField.GetRawConstantValue().ToString();
                        if (key == name)
                        {
                            return defaultValue;
                        }
                    }
                }
            }
            return result;
        }
    }
}
