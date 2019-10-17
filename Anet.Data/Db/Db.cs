using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace Anet.Data
{
    public class Db
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
            // See: Implement IDisposable correctly
            // https://docs.microsoft.com/en-us/visualstudio/code-quality/ca1063-implement-idisposable-correctly

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Transaction != null)
                {
                    Transaction.Dispose();
                    Transaction = null;
                }
                if (Connection != null)
                {
                    Connection.Dispose();
                    Connection = null;
                }
            }

            // free native resources if there are any.
        }

        ~Db()
        {
            Dispose(false);
        }
    }
}
