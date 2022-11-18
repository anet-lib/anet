namespace Anet.Models;

public class PagedResult<T>
{
    public PagedResult()
    {
    }

    public PagedResult(int page, int size)
    {
        Page = page;
        Size = size;
    }

    public int Page { get; set; }
    public int Size { get; set; }
    public int Total { get; set; }
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public T Summary { get; set; }

    public int TotalPages => Size == 0 ? 0 : (int)Math.Ceiling((decimal)Total / Size);
}
