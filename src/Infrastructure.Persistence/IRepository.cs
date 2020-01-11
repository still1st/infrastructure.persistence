using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public interface IRepository<T> where T: class
    {
        Task<long> CountAsync();
        Task<long> CountAsync(ISpecification<T> specification);

        Task<bool> AnyAsync();
        Task<bool> AnyAsync(ISpecification<T> specification);

        Task<T> GetOneAsync(ISpecification<T> specification);
        Task<List<T>> GetManyAsync(ISpecification<T> specification);
        Task<List<T>> GetManyAsync(ISpecification<T> specification, ISorting<T> sorting);

        Task<List<T>> GetManyAsync(ISpecification<T> specification, ISorting<T> sorting,
            Limiting limit);

        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);

        Task DeleteAsync(ISpecification<T> specification);
    }
}
