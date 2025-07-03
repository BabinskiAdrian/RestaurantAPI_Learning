namespace RestaurantAPI.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalPages { get; set; }
        public int ItemsFrom { get; set; }
        public int ItemsTo { get; set; }   
        public int TotalItemsCount { get; set; }

        public PagedResult() { } 

        public PagedResult(List<T>items, int totalCount, int pageSize, int PageNumber)
        { 
            Items = items;
            TotalItemsCount = totalCount;
            ItemsFrom = (PageNumber - 1) + 1;
            ItemsTo = ItemsFrom + pageSize -1;
            TotalPages = (int)Math.Ceiling(totalCount/ (double)pageSize);

        }
    }
}
