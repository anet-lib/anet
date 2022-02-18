using Anet;
using Anet.Data;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
    /// <summary>
    /// Adds Anet services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="setup">Setup the options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAnet(this IServiceCollection services,
        Action<AnetOptions> setup = null)
    {
        var options = new AnetOptions();
        setup?.Invoke(options);

        return services;
    }

    /// <summary>
    /// Adds database services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of a db provider's connection.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="configure">Configure the provided <see cref="DbOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAnetDb<TDbConnection>(this IServiceCollection services,
        string connectionString,
        Action<DbOptions> configure = null)
        where TDbConnection : IDbConnection, new()
    {
        return services.AddAnetDb<Db, TDbConnection>(connectionString, configure);
    }

    /// <summary>
    /// Adds database services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDb">The custom type of <see cref="Db"/>.</typeparam>
    /// <typeparam name="TDbConnection">The type of a db provider's connection.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="configure">Configure the provided <see cref="DbOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAnetDb<TDb, TDbConnection>(this IServiceCollection services,
        string connectionString,
        Action<DbOptions> configure = null)
        where TDb : Db
        where TDbConnection : IDbConnection, new()
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        return services.AddAnetDb<TDb>(
            _ => new TDbConnection
            {
                ConnectionString = connectionString
            },
            configure);
    }

    /// <summary>
    /// Adds database services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDb">The custom type of <see cref="Db"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="connectionFactory">The <see cref="IDbConnection"/> factory.</param>
    /// <param name="configure">Configure the provided <see cref="DbOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAnetDb<TDb>(this IServiceCollection services,
        Func<IServiceProvider, IDbConnection> connectionFactory,
        Action<DbOptions> configure = null)
        where TDb : Db
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        var options = new DbOptions();
        configure?.Invoke(options);

        TDb implFactory(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<Db>>();
            var connection = connectionFactory(serviceProvider);
            return (TDb)Activator.CreateInstance(typeof(TDb), connection, options, logger);
        }

        services.AddScoped(implFactory);

        return services;
    }

    ///// <summary>
    ///// Adds generic repository services to the specified <see cref="IServiceCollection"/>. 
    ///// </summary>
    ///// <param name="services"></param>
    ///// <returns></returns>
    //public static IServiceCollection AddRepository(this IServiceCollection services)
    //{
    //    services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
    //    services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
    //    return services;
    //}
}
