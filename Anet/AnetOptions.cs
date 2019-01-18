namespace Anet
{
    public class AnetOptions
    {
        /// <summary>
        /// Use a default instance of <see cref="IdGen"/>.
        /// </summary>
        /// <param name="idGen">The instace of <see cref="IdGen"/>.</param>
        public void UseIdGen(IdGen idGen = null)
        {
            IdGen.SetDefault(idGen);
        }
    }
}
