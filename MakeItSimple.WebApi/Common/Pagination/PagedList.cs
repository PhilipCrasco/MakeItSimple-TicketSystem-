using MakeItSimple.WebApi.DataAccessLayer.Feature.UserFeatures;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.Common.Pagination
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get;  private set; }

        public bool HasPreviousPage { get; private set; }

        public bool HasNextPage { get; private set; }
        public bool IsFailure { get; internal set; }

        public PagedList(List<T> items , int count , int pageNumber , int pageSize)
        {
            
            TotalCount = count ;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
            
        }

        public async static Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        internal static Task<PagedList<GetUser.GetUserResult>> CreateAsync(List<GetUser.GetUserResult> users, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
