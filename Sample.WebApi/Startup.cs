using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.WebApi.Repositories;
using Sample.WebApi.Services;
using System.Data.SqlClient;

namespace Sample.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabase<SqlConnection>(Configuration.GetConnectionString("DefaultConnection"));

            services.AddAnetMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddTransient<UserRepository>();
            services.AddTransient<UserService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAnet();
            app.UseMvc();
        }
    }
}
