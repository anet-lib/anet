using Anet;
using Anet.Data;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Microsoft.Extensions.DependencyInjection;

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

    /// <summary>
    /// Adds database services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of a db provider's connection.</typeparam>
    /// <param name="builder">The <see cref="AnetBuilder"/> to add services to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddDb<TDbConnection>(this AnetBuilder builder, DbDialect dialect, string connectionString)
        where TDbConnection : IDbConnection, new()
    {
        return builder.AddDb<Db, TDbConnection>(dialect, connectionString);
    }

    /// <summary>
    /// Adds database services to the specified <see cref="AnetBuilder"/>.
    /// </summary>
    /// <typeparam name="TDb">The custom type of <see cref="Db"/>.</typeparam>
    /// <typeparam name="TDbConnection">The type of a db provider's connection.</typeparam>
    /// <param name="builder">The <see cref="AnetBuilder"/> to add services to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddDb<TDb, TDbConnection>(this AnetBuilder builder, DbDialect dialect, string connectionString)
        where TDb : Db
        where TDbConnection : IDbConnection, new()
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        return builder.AddDb<TDb>(dialect, _ => new TDbConnection
        {
            ConnectionString = connectionString
        });
    }

    /// <summary>
    /// Adds database services to the specified <see cref="AnetBuilder"/>.
    /// </summary>
    /// <typeparam name="TDb">The custom type of <see cref="Db"/>.</typeparam>
    /// <typeparam name="TDbConnection">The type of a db provider's connection.</typeparam>
    /// <param name="builder">The <see cref="AnetBuilder"/> to add services to.</param>
    /// <param name="setup">Setup the options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddDb<TDb, TDbConnection>(this AnetBuilder builder, Action<DbOptions> setup)
        where TDb : Db
        where TDbConnection : IDbConnection, new()
    {
        ArgumentNullException.ThrowIfNull(setup);

        var options = new DbOptions();
        setup.Invoke(options);

        ArgumentNullException.ThrowIfNull(options.ConnectionString);

        return builder.AddDb<TDb, TDbConnection>(options.DbDialect, options.ConnectionString);
    }

    /// <summary>
    /// Adds database services to the specified <see cref="AnetBuilder"/>.
    /// </summary>
    /// <typeparam name="TDb">The custom type of <see cref="Db"/>.</typeparam>
    /// <param name="builder">The <see cref="AnetBuilder"/> to add services to.</param>
    /// <param name="connectionFactory">The <see cref="IDbConnection"/> factory.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddDb<TDb>(this AnetBuilder builder, DbDialect dialect, Func<IServiceProvider, IDbConnection> connectionFactory)
        where TDb : Db
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        Sql.DefaultDialect = dialect;

        TDb implFactory(IServiceProvider serviceProvider)
        {
            // a logger for db access
            if (Db.Logger == null)
            {
                Db.Logger = serviceProvider.GetService<ILogger<Db>>();
            }

            var connection = connectionFactory(serviceProvider);

            return (TDb)Activator.CreateInstance(typeof(TDb), connection);
        }

        builder.Services.AddScoped(implFactory);

        return builder;
    }

    ///// <summary>
    ///// Adds generic repository services to the specified <see cref="IServiceCollection"/>. 
    ///// </summary>
    ///// <param name="builder"></param>
    ///// <returns></returns>
    //public static AnetBuilder AddRepository(this AnetBuilder builder)
    //{
    //    builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
    //    builder.Services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
    //    return builder;
    //}
}
