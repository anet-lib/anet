using Anet.Data;
using Microsoft.AspNetCore.Builder;
using Sample.Web.Services;
using System.Data.SqlClient;

namespace Sample.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connStr = Configuration.GetConnectionString("DefaultConnection");

            services.AddAnet().AddDb<SqlConnection>(DbDialect.SQLServer, connStr);
            //services.AddAnet().AddDb<MySqlConnector.MySqlConnection>(DbDialect.MySQL, connStr);

            services.AddControllers();

            services.AddTransient<UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
