namespace MakeItSimple.WebApi.Common.Pagination
{
    public class UserParams
    {
        private const int MaxPageSize = 10000;

        public int PageNumber { get; set; } = 1;
        public int _pageSize = MaxPageSize;

        public int PageSize 
        {
           get => _pageSize;
           set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value ;
        }

    }
}
