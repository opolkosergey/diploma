using System;
using System.Collections.Generic;
using System.Linq;

namespace Diploma.Pagging
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; }
        public int TotalPages { get; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = CalculateTotalPages(count, pageSize);

            AddRange(items);
        }

        private int CalculateTotalPages(int count, int pageSize) => (int)Math.Ceiling(count / (double)pageSize);      

        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, source.Count(), pageIndex, pageSize);
        }
    }
}
