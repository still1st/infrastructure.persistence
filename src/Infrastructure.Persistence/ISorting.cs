using System;
using System.Linq.Expressions;

namespace Infrastructure.Persistence
{
    public interface ISorting<T>
    {
        Expression<Func<T, object>> Selector { get; }

        SortingType SortingType { get; }
    }

    public class Sorting<T>: ISorting<T>
    {
        public Expression<Func<T, object>> Selector { get; }

        public SortingType SortingType { get; }

        public Sorting(Expression<Func<T, object>> selector, SortingType sortingType = SortingType.Ascending)
        {
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            SortingType = sortingType;
        }
    }
}
