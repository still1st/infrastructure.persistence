namespace Infrastructure.Persistence
{
    public class PaginationQuery
    {
        public const int DefaultPageSize = 10;
        public const int DefaultPageIndex = 1;

        public int PageIndex { get; set; } = DefaultPageIndex;
        public int PageSize { get; set; } = DefaultPageSize;
    }
}
