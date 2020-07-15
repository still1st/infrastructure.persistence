using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class Repository<T>: IRepository<T> where T : class
    {
        protected readonly DbSet<T> DbSet;

        public Repository(DbContext dbContext)
        {
            DbSet = dbContext.Set<T>();
        }

        public void Add(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await DbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException(nameof(entities));

            await DbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException(nameof(entities));

            DbSet.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Remove(entity);
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
            => await DbSet.CountAsync(spec.Predicate);

        public async Task<bool> AnyAsync(ISpecification<T> spec)
            => await DbSet.AnyAsync(spec.Predicate);

        public async Task<T> GetOneAsync(ISpecification<T> spec)
        {
            var query = DbSet.Include(spec.Includes);

            return spec.AsNoTracking ?
                await query.AsNoTracking().FirstOrDefaultAsync(spec.Predicate) :
                await query.FirstOrDefaultAsync(spec.Predicate);
        }

        public async Task<List<T>> GetManyAsync(ISpecification<T> spec)
        {
            var query = DbSet.Include(spec.Includes);

            if (spec.Predicate != null)
                query = query.Where(spec.Predicate);

            return spec.AsNoTracking ?
                await query.AsNoTracking().ToListAsync() :
                await query.ToListAsync();
        }

        public async Task<List<T>> GetManyAsync(ISpecification<T> spec, Sorting sorting)
        {
            var query = GetQuery(
                spec: spec,
                filter: null,
                sorting: sorting);

            return await query.ToListAsync();
        }

        public async Task<PaginatedList<TResult>> GetPaginatedListAsync<TResult>(ISpecification<T> spec, 
            PaginationQuery parameters,
            Func<IQueryable<T>, IQueryable<TResult>> projection,
            Expression<Func<T, bool>>? filter = null,
            Sorting? sorting = null)
        {
            var query = GetQuery(spec, projection, filter, sorting);
            var result = await PaginatedList<TResult>.CreateAsync(query, parameters.PageIndex, parameters.PageSize);
            return result;
        }

        private IQueryable<TResult> GetQuery<TResult>(ISpecification<T> spec,
            Func<IQueryable<T>, IQueryable<TResult>> projection,
            Expression<Func<T, bool>>? filter = null,
            Sorting? sorting = null)
        {
            var query = GetQuery(spec, filter, sorting);
            return projection(query);
        }

        private IQueryable<T> GetQuery(ISpecification<T> spec,
            Expression<Func<T, bool>>? filter = null,
            Sorting? sorting = null)
        {
            var query = DbSet.Include(spec.Includes);

            if (spec.Predicate != null)
                query = query.Where(spec.Predicate);

            // filter that comes from UI
            if (filter != null)
                query = query.Where(filter);

            if (sorting != null)
            {
                query = sorting.SortType == SortingType.Asc
                    ? query.OrderBy(sorting.SortBy)
                    : query.OrderByDescending(sorting.SortBy);
            }

            if (spec.AsNoTracking)
                query = query.AsNoTracking();

            return query;
        }

        public async Task DeleteAsync(ISpecification<T> spec)
        {
            if (spec is null)
                throw new ArgumentNullException(nameof(spec));

            var entities = await DbSet.Where(spec.Predicate).ToListAsync();
            DbSet.RemoveRange(entities);
        }
    }
}
