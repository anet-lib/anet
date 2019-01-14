using Anet.Web;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AnetWebServiceCollectionExtensions
    {
        public static IMvcBuilder AddAnetMvc(this IServiceCollection services, Action<MvcOptions> setupAction = null)
        {
            return services.AddMvc(options =>
            {
                options.Filters.Add(new ExceptionHandlerFilterAttribute());
                options.Filters.Add(new ModelStateValidationFilterAttribute());
                setupAction?.Invoke(options);
            });
        }
    }
}
