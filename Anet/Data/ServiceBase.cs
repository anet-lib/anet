namespace Anet.Data;

/// <summary>
/// A base class for a business logic service.
/// </summary>
/// <typeparam name="TDb">The child type of <see cref="Data.Db"/></typeparam>
public abstract class ServiceBase<TDb> where TDb : Db
{
    /// <summary>
    /// Initialize the base class of a service.
    /// </summary>
    /// <param name="db">The database to access.</param>
    public ServiceBase(TDb db)
    {
        Db = db;
    }

    /// <summary>
    /// The database to access.
    /// </summary>
    public TDb Db { get; }
}

/// <summary>
/// A base class for a business logic service.
/// </summary>
public abstract class ServiceBase : ServiceBase<Db>
{
    /// <summary>
    /// Initialize the base class of a service.
    /// </summary>
    /// <param name="db">The database to access.</param>
    protected ServiceBase(Db db) : base(db)
    {
    }
}
