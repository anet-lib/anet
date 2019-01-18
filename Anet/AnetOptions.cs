namespace Anet
{
    public class AnetOptions
    {
        /// <summary>
        /// Set the default instance of <see cref="IdGen"/>.
        /// </summary>
        /// <param name="idGen">The instace of <see cref="IdGen"/>.</param>
        public void SetIdGen(IdGen idGen = null)
        {
            IdGen.SetDefault(idGen);
        }
    }
}
