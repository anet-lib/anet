using Anet;
using Anet.Data;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class AnetBuilderExtensions
{
    /// <summary>
    /// Adds database services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDb">The custom type of <see cref="Db"/>.</typeparam>
    /// <param name="builder">The <see cref="AnetBuilder"/> to add services to.</param>
    /// <param name="dbConnectionFactory">The <see cref="IDbConnection"/> factory.</param>
    /// <returns>The <see cref="AnetDataBuilder"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddDb<TDb>(this AnetBuilder builder, Func<IServiceProvider, IDbConnection> dbConnectionFactory)
        where TDb : Db
    {
        TDb implementationFactory(IServiceProvider serviceProvider)
        {
            if (Db.Logger == null)
            {
                Db.Logger = serviceProvider.GetService<ILogger<Db>>();
            }

            var connection = dbConnectionFactory(serviceProvider);

            return (TDb)Activator.CreateInstance(typeof(TDb), connection);
        }

        builder.Services.AddScoped(implementationFactory);

        if (typeof(TDb) != typeof(Db))
        {
            builder.Services.AddScoped<Db>(implementationFactory);
        }

        return builder;
    }

    /// <summary>
    /// Adds database services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDb">The custom type of <see cref="Db"/>.</typeparam>
    /// <typeparam name="TDbConnection">The type of a db provider's connection.</typeparam>
    /// <param name="builder">The <see cref="AnetDataBuilder"/> to add services to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The <see cref="AnetDataBuilder"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddDb<TDb, TDbConnection>(this AnetBuilder builder, string connectionString)
        where TDb : Db
        where TDbConnection : IDbConnection, new()
    {
        return builder.AddDb<TDb>(_ => new TDbConnection
        {
            ConnectionString = connectionString
        });
    }

    /// <summary>
    /// Adds database services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of a db provider's connection.</typeparam>
    /// <param name="builder">The <see cref="AnetDataBuilder"/> to add services to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The <see cref="AnetDataBuilder"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddDb<TDbConnection>(this AnetBuilder builder, string connectionString)
        where TDbConnection : IDbConnection, new()
    {
        return builder.AddDb<Db, TDbConnection>(connectionString);
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
