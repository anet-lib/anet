using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Anet.Web;

public class AnetWebBuilder
{
    public AnetWebBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; private set; }

    public IMvcBuilder MvcBuilder { get; internal set; }

    public AuthenticationBuilder AuthenticationBuilder { get; internal set; }
}

