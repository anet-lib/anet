using System;
using System.Threading.Tasks;

namespace Anet.JobApp
{
    public interface IJob
    {
        Task ExecuteAsync();
        Task OnExceptionAsync(Exception ex);
    }
}
