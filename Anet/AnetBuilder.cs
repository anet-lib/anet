using Microsoft.Extensions.DependencyInjection;

namespace Anet
{
    public class AnetBuilder
    {
        public AnetBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; private set; }
    }
}
