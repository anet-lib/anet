using Anet.Web;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AnetWebServiceCollectionExtensions
    {
        public static IMvcCoreBuilder AddAnetMvcCore(this IServiceCollection services, Action<MvcOptions> setupAction = null)
        {
            return services.AddMvcCore(options => {
                options.Filters.Add(new ExceptionHandlerFilterAttribute());
                options.Filters.Add(new ModelStateValidationFilterAttribute());
                setupAction(options);
            });
        }
    }
}
