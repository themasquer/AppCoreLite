namespace AppCoreLite.Models
{
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
