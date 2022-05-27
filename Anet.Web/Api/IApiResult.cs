namespace Anet.Web.Api;

public interface IApiResult
{
    public ushort Code { get; set; }
    public string Message { get; set; }
}
