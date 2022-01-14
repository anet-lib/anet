namespace Microsoft.AspNetCore.Hosting;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder UsePortFromArgs(
        this IWebHostBuilder builder, string[] args, int defaultPort = 5000, string portArgName = "port")
    {
        var port = GetArgValue(args, portArgName, defaultPort.ToString());
        builder.UseUrls($"http://127.0.0.1:{port}");
        return builder;
    }

    private static string GetArgValue(string[] args, string name, string defautValue = "")
    {
        var keyValue = args.Where(x => x.StartsWith($"{name}=")).FirstOrDefault();
        if (keyValue != null)
            return keyValue.Split('=')[1];
        return defautValue;
    }
}

