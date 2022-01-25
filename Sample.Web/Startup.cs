using Anet.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
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

        public void ConfigureServices(IServiceCollection services)
        {
            var connStr = Configuration.GetConnectionString("DefaultConnection");

            // This just for setup demo database.
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connStr));

            services.AddAnet().AddDb<SqlConnection>(DbDialect.SQLServer, connStr);
            //services.AddAnet().AddDb<MySqlConnector.MySqlConnection>(DbDialect.MySQL, connStr);

            services.AddControllers();

            services.AddTransient<UserService>();
        }

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
