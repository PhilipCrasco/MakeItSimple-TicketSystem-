namespace MakeItSimple.WebApi.Common.Pagination
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }

        public int ItemPerPage { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public bool HasPrevious {  get; set; }

        public bool HasNext { get; set; }

        public PaginationHeader(int currentPage , int itemsPerPage , int totalItems , int totalPages , bool hasPrevious , bool hasNext )
        {

            CurrentPage = currentPage;
            ItemPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
            HasPrevious = hasPrevious;
            HasNext = hasNext;
            
        }
    }


}
