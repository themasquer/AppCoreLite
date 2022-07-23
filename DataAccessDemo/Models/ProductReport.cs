using AppCoreLite.Attributes;
using AppCoreLite.Models.Bases;
using System.ComponentModel;

namespace DataAccessDemo.Models
{
    public class ProductReport : ReportBase
    {
        [DisplayName("Category Name")]
        [OrderTag]
        [ExportTag]
        public string? CategoryName { get; set; }

        [DisplayName("Product Name")]
        [OrderTag]
        [ExportTag]
        public string? ProductName { get; set; }

        public string? ProductDescription { get; set; }

        public string? CategoryDescription { get; set; }

        [DisplayName("Unit Price")]
        [ExportTag]
        public string? UnitPriceDisplay { get; set; }

        [DisplayName("Stock Amount")]
        [OrderTag]
        [ExportTag]
        public int? StockAmount { get; set; }

        [DisplayName("Expiration Date")]
        [ExportTag]
        public string? ExpirationDateDisplay { get; set; }

        [DisplayName("Store Name")]
        [OrderTag]
        [ExportTag]
        public string? StoreName { get; set; }



        public int? CategoryId { get; set; }

        [OrderTag]
        [DisplayName("Unit Price")]
        public double? UnitPrice { get; set; }

        [OrderTag]
        [DisplayName("Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        public int? StoreId { get; set; }
    }
}
