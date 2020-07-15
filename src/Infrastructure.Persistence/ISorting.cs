namespace Infrastructure.Persistence
{
    public class Sorting
    {
        public string SortBy { get; }
        public SortingType SortType { get; }

        public Sorting(string sortBy, SortingType sortType = SortingType.Asc)
        {
            SortBy = sortBy;
            SortType = sortType;
        }
    }

    public enum SortingType
    {
        Asc = 1,
        Desc = 2
    }
}
