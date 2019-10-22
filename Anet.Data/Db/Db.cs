using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace Anet.Data
{
    public class Db : IDisposable
    {
        internal static ILogger<Db> Logger { get; set; }

        public Db(IDbConnection connection)
        {
            Connection = connection;
        }

        public IDbConnection Connection { get; private set; }

        public IDbTransaction Transaction { get; private set; }

        public IDbTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.Unspecified);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            // Auto open connection.
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
            Transaction = Connection.BeginTransaction(il);
            return Transaction;
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Transaction = null;
            Connection?.Dispose();
            Connection = null;
        }
    }
}
