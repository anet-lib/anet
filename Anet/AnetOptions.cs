namespace Anet
{
    public class AnetOptions
    {
        /// <summary>
        /// Set the default instance of <see cref="IdGenerator"/>.
        /// </summary>
        /// <param name="idGenerator">The instace of <see cref="IdGenerator"/>.</param>
        public void SetIdGenerator(IdGenerator idGenerator = null)
        {
            IdGen.SetInstace(idGenerator);
        }
    }
}
