namespace Anet.Data
{
    public class PageQuery
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; }
        public string Keyword { get; set; }
        public bool EnableTotal { get; set; }
    }
}
