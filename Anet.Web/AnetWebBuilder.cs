using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Anet.Web;

public class AnetWebBuilder : AnetBuilder
{
    public AnetWebBuilder(IServiceCollection services) : base(services)
    { }

    public IMvcBuilder MvcBuilder { get; internal set; }

    public AuthenticationBuilder AuthenticationBuilder { get; internal set; }
}

