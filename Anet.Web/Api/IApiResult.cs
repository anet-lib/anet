namespace Anet.Web.Api;

public interface IApiResult
{
    public short Code { get; set; }
    public string Message { get; set; }
}
