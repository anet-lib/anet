using Anet.Data;
using Microsoft.Extensions.Logging;

namespace Anet
{
    public class AnetOptions
    {
        /// <summary>
        /// Use <see cref="IdGen"/> for generating sequence IDs.
        /// </summary>
        /// <param name="machineId">An unique id of current machine.</param>
        public void UseIdGen(ushort machineId)
        {
            IdGen.SetMachineId(machineId);
        }

        /// <summary>
        /// User a logger factory to create a logger for database accessing.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public void UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            Db.Logger = loggerFactory.CreateLogger<Db>();
        }
    }
}
