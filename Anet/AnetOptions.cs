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
    }
}
