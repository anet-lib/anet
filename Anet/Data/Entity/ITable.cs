using System;

namespace Anet.Data.Entity
{
    /// <summary>
    /// Corresponds to a database table.
    /// </summary>
    public interface ITable
    {
        DateTime CreatedAt { get; set; }
    }
}
