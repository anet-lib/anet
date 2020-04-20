using Anet;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AnetBuilderExtensions
    {
        /// <summary>
        /// Adds Anet services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="setup">Setup the options.</param>
        /// <returns>The <see cref="AnetBuilder"/> so that additional calls can be chained.</returns>
        public static AnetBuilder AddAnet(this IServiceCollection services, Action<AnetOptions> setup = null)
        {
            var options = new AnetOptions();
            setup?.Invoke(options);

            return new AnetBuilder(services);
        }
    }
}
