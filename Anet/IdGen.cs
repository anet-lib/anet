using System;

namespace Anet
{
    /// <summary>
    /// Shortcut for default instance of <see cref="IdGenerator"/>.
    /// </summary>
    public static class IdGen
    {
        private static IdGenerator Instance { get; set; }

        /// <summary>
        /// Set the default instance of <see cref="IdGenerator"/>.
        /// </summary>
        /// <param name="idGenerator">The instace of <see cref="IdGenerator"/>.</param>
        public static void SetInstace(IdGenerator idGenerator = null)
        {
            if (Instance != null)
                throw new InvalidOperationException("The default IdGenerator instace can't be set twice.");
            Instance = idGenerator ?? new IdGenerator();
        }

        /// <summary>
        /// Generate a new sequence id.
        /// </summary>
        /// <returns>The generated id.</returns>
        public static long NewId()
        {
            if (Instance == null)
                throw new Exception("The IdGen has no IdGenerator instance.");
            return Instance.NewId();
        }
    }
}
