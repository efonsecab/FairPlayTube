using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace FairPlayTube.Swagger.Filters
{
    /// <summary>
    /// Used to add the required configuration to be able to use Swagger UI with Authentcation
    /// </summary>
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the this filter on swagger documentation generation.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // then check if there is a method-level 'AllowAnonymous', as this overrides any controller-level 'Authorize'
            var anonControllerScope = context
                    .MethodInfo
                    .DeclaringType
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>();

            var anonMethodScope = context
                    .MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>();

            // only add authorization specification information if there is at least one 'Authorize' in the chain and NO method-level 'AllowAnonymous'
            if (!anonMethodScope.Any() && !anonControllerScope.Any())
            {
                // add generic message if the controller methods dont already specify the response type
                if (!operation.Responses.ContainsKey("401"))
                    operation.Responses.Add("401", new OpenApiResponse { Description = "If Authorization header not present, has no value or no valid jwt bearer token" });

                if (!operation.Responses.ContainsKey("403"))
                    operation.Responses.Add("403", new OpenApiResponse { Description = "If user not authorized to perform requested action" });

                var jwtAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [ jwtAuthScheme ] = new List<string>()
                }
            };
            }
        }
    }
}
