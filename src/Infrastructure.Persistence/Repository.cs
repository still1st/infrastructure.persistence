using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Persistence
{
    public class Repository<T>: IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<long> CountAsync() => await _dbSet.CountAsync();

        public async Task<long> CountAsync(ISpecification<T> specification)
            => await _dbSet.CountAsync(specification.Predicate);

        public async Task<bool> AnyAsync() => await _dbSet.AnyAsync();

        public async Task<bool> AnyAsync(ISpecification<T> specification)
            => await _dbSet.AnyAsync(specification.Predicate);

        public async Task<T> GetOneAsync(ISpecification<T> specification)
        {
            var query = _dbSet.Include(specification.Includes);

            return specification.AsNoTracking ?
                await query.AsNoTracking().FirstOrDefaultAsync(specification.Predicate) :
                await query.FirstOrDefaultAsync(specification.Predicate);
        }

        public async Task<List<T>> GetManyAsync(ISpecification<T> specification)
        {
            var query = _dbSet.Include(specification.Includes);

            if (specification.Predicate != null)
                query = query.Where(specification.Predicate);

            return specification.AsNoTracking ?
                await query.AsNoTracking().ToListAsync() :
                await query.ToListAsync();
        }

        public async Task<List<T>> GetManyAsync(ISpecification<T> specification, ISorting<T> sorting)
        {
            var query = _dbSet.Include(specification.Includes);

            if (specification.Predicate != null)
                query = query.Where(specification.Predicate);

            query = sorting.SortingType == SortingType.Ascending ?
                query.OrderBy(sorting.Selector) :
                query.OrderByDescending(sorting.Selector);

            return specification.AsNoTracking ?
                await query.AsNoTracking().ToListAsync() :
                await query.ToListAsync();
        }

        public async Task<List<T>> GetManyAsync(ISpecification<T> specification, ISorting<T> sorting,
            Limiting limit)
        {
            var query = _dbSet.Include(specification.Includes);

            if (specification.Predicate != null)
                query = query.Where(specification.Predicate);

            query = sorting.SortingType == SortingType.Ascending ?
                query.OrderBy(sorting.Selector) :
                query.OrderByDescending(sorting.Selector);

            return specification.AsNoTracking ?
                await query.Take(limit.CountOfRecords).AsNoTracking().ToListAsync() :
                await query.Take(limit.CountOfRecords).ToListAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteAsync(ISpecification<T> specification)
        {
            var entitiesToDelete = await GetManyAsync(specification);
            _dbSet.RemoveRange(entitiesToDelete);
        }
    }

    public static class IQueryableExtensions
    {
        /// <summary>
        /// Includes an array of navigation properties for the specified query.
        /// </summary>
        /// <typeparam name="T">The type of the entity</typeparam>
        /// <param name="query">The query to include navigation properties for that</param>
        /// <param name="includes">The array of navigation properties to include</param>
        /// <returns></returns>
        public static IQueryable<T> Include<T>(this IQueryable<T> query,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes)
            where T : class
        {
            if (includes == null)
                return query;

            foreach (var include in includes)
                query = include(query);

            return query;
        }
    }
}
