namespace Anet
{
    public interface IPagination
    {
        public int Total { get; set; }
        public int Size { get; set; }
        public int Page { get; set; }
        public int TotalPages { get;  }
    }
}
