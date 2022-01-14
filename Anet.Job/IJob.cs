namespace Anet.Job;

public interface IJob
{
    Task ExecuteAsync();
    Task OnExceptionAsync(Exception ex);
}

