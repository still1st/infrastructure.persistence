using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        void Delete(T entity);
        Task DeleteAsync(ISpecification<T> spec);

        Task<int> CountAsync(ISpecification<T> spec);
        Task<bool> AnyAsync(ISpecification<T> spec);

        Task<T> GetOneAsync(ISpecification<T> spec);
        Task<List<T>> GetManyAsync(ISpecification<T> spec);
        Task<List<T>> GetManyAsync(ISpecification<T> spec, Sorting sorting);

        Task<PaginatedList<TResult>> GetPaginatedListAsync<TResult>(ISpecification<T> spec,
            PaginationQuery parameters,
            Func<IQueryable<T>, IQueryable<TResult>> projection,
            Expression<Func<T, bool>>? filter = null,
            Sorting? sorting = null);
    }
}
