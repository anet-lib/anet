namespace Anet.Web;

public interface IApiResult
{
    public int Code { get; set; }
    public string Message { get; set; }
}
