namespace Anet;

public partial class IdGen
{
    private static IdGen _instance;

    internal static void SetDefaultOptions(Action<IdGenOptions> configure)
    {
        if (_instance != null)
            throw new InvalidOperationException("Can't set machine id twice.");

        var options = new IdGenOptions();
        configure?.Invoke(options);

        _instance = new IdGen(options);
    }

    /// <summary>
    /// Generate a new sequence id.
    /// </summary>
    /// <returns>The generated id.</returns>
    public static long NewId()
    {
        if (_instance == null)
            throw new Exception("The IdGen has no default instance.");
        return _instance.NewSequenceId();
    }

    
}
