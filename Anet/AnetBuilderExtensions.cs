using Anet;
using Anet.Data;
using Anet.Data.Logging;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace Microsoft.Extensions.DependencyInjection;

public static class AnetBuilderExtensions
{
    /// <summary>
    /// Adds Anet services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="setup">Setup the options.</param>
    /// <returns>The <see cref="AnetBuilder"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddAnet(this IServiceCollection services,
        Action<AnetOptions> setup = null)
    {
        var options = new AnetOptions();
        setup?.Invoke(options);

        return new AnetBuilder(services);
    }

    /// <summary>
    /// 是否启用默认的ID生成器
    /// </summary>
    /// <param name="machineId">当前机器码（唯一机会编号）</param>
    /// <param name="machineIdBits">机器码位数（0-10之间）</param>
    /// <param name="sequenceBits">
    /// 序列号位数（值在0-20之间）
    /// 注意：
    /// 1. 并发量越大，此值也要越大，例如：10 可以 1 秒内生成 2^10=1024 个 ID。
    /// 2. 每台机器此参数务必相同。
    /// </param>
    public static AnetBuilder EnableDefaultIdGen(this AnetBuilder builder,
        ushort machineId,
        byte machineIdBits = IdGen.DefaultMachineIdBits,
        byte sequenceBits = IdGen.DefaultSequenceBits)
    {
        IdGen.Init(machineId, machineIdBits, sequenceBits);
        return builder;
    }

    /// <summary>
    /// Adds database services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of a db provider's connection.</typeparam>
    /// <param name="builder">The <see cref="AnetBuilder"/> to add services to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddDb<TDbConnection>(this AnetBuilder builder,
        DbDialect dialect,
        string connectionString)
        where TDbConnection : DbConnection, new()
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
    public static AnetBuilder AddDb<TDb, TDbConnection>(this AnetBuilder builder,
        DbDialect dialect,
        string connectionString)
        where TDb : Db
        where TDbConnection : DbConnection, new()
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
    /// <param name="builder">The <see cref="AnetBuilder"/> to add services to.</param>
    /// <param name="connectionFactory">The <see cref="IDbConnection"/> factory.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static AnetBuilder AddDb<TDb>(this AnetBuilder builder,
        DbDialect dialect,
        Func<IServiceProvider, DbConnection> connectionFactory,
        Action<DbOptions> configure = null)
        where TDb : Db
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        var options = new DbOptions();
        configure?.Invoke(options);

        TDb implFactory(IServiceProvider serviceProvider)
        {
            var connection = connectionFactory(serviceProvider);
            var logger = serviceProvider.GetService<ILogger<Db>>();
            var hooks = new LoggingHooks(logger, options.LoggingOptions ?? new LoggingOptions());
            var anetConnection = new AnetDbConnection(connection, hooks)
            {
                MetricsEnabled = options.EnableMetrics
            };

            return (TDb)Activator.CreateInstance(typeof(TDb), dialect, anetConnection);
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
