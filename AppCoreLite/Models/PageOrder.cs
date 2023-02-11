namespace AppCoreLite.Models
{
    public class PageOrder
    {
        public int PageNumber { get; set; }
        public string RecordsPerPageCount { get; set; }
        public int TotalRecordsCountResult { get; set; }
        public string OrderExpression { get; set; }
        public bool IsOrderDirectionAscending { get; set; }

        public PageOrder(int pageNumber = 1, string recordsPerPageCount = "5", string orderExpression = "", bool isOrderDirectionAscending = true)
        {
            PageNumber = pageNumber;
            RecordsPerPageCount = recordsPerPageCount;
            OrderExpression = orderExpression;
            IsOrderDirectionAscending = isOrderDirectionAscending;
        }
    }

    public class PageOrderFilter : PageOrder
    {
        public string Filter { get; set; }

        public PageOrderFilter(int pageNumber = 1, string recordsPerPageCount = "5", string orderExpression = "", bool isOrderDirectionAscending = true, string filter = "")
            : base(pageNumber, recordsPerPageCount, orderExpression, isOrderDirectionAscending)
        {
            Filter = filter;
        }
    }
}
