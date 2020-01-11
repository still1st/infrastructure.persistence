using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Persistence
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Predicate { get; }

        bool AsNoTracking { get; }

        Func<IQueryable<T>, IIncludableQueryable<T, object>>[] Includes { get; }
    }
}
