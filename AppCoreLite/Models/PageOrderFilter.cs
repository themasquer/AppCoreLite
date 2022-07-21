namespace AppCoreLite.Models
{
    public class PageOrderFilter
    {
        public int PageNumber { get; set; }
        public string RecordsPerPageCount { get; set; }
        public int TotalRecordsCountResult { get; set; }
        public string OrderExpression { get; set; }
        public bool IsOrderDirectionAscending { get; set; }
        public string Filter { get; set; }

        public PageOrderFilter(int pageNumber = 1, string recordsPerPageCount = "5", string orderExpression = "", bool isOrderDirectionAscending = true, string filter = "")
        {
            PageNumber = pageNumber;
            RecordsPerPageCount = recordsPerPageCount;
            OrderExpression = orderExpression;
            IsOrderDirectionAscending = isOrderDirectionAscending;
            Filter = filter;
        }
    }
}
