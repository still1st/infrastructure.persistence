using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var skip = Math.Max((pageIndex - 1) * pageSize, 0);

            var count = await source.CountAsync();
            var items = await source.Skip(skip).Take(pageSize).ToListAsync();

            return Create(pageIndex, pageSize, items, count);
        }

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var skip = Math.Max((pageIndex - 1) * pageSize, 0);

            var count = source.Count();
            var items = source.Skip(skip).Take(pageSize).ToList();

            return Create(pageIndex, pageSize, items, count);
        }

        private static PaginatedList<T> Create(int pageIndex, int pageSize, List<T> items, int totalItems)
            => new PaginatedList<T>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                TotalItems = totalItems,
            };
    }
}
