using System;

namespace Anet.Data
{
    /// <summary>
    /// Corresponds to a database table.
    /// </summary>
    public interface ITable
    {
        DateTime CreatedAt { get; set; }
    }
}
