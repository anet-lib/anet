using System.Text.Json;
using Anet;
using Anet.Utilities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
    /// <summary>
    /// Adds Anet services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAnet(this IServiceCollection services,
        Action<AnetOptions> configureAnet = null,
        Action<IdGenOptions> configureIdGen = null,
        Action<JsonSerializerOptions> configureJsonUtil = null)
    {
        var options = new AnetOptions();
        configureAnet?.Invoke(options);

        IdGen.SetDefaultOptions(configureIdGen);
        JsonUtil.SetDefaultSerializerOptions(configureJsonUtil);

        return services;
    }
}
