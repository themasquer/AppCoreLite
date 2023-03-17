using AppCoreLite.ViewModels;
using DataAccessDemo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace MvcDemo.Models
{
    public class ProductsReportIndexViewModel : PageOrderViewModel
    {
        public List<ProductReportModel> Products { get; set; }
        public MultiSelectList Categories { get; set; }
        public MultiSelectList Stores { get; set; }

        [DisplayName("Category")]
        public int? CategoryId { get; set; }

        [DisplayName("Product")]
        public string? ProductName { get; set; }

        [DisplayName("Unit Price")]
        public double? UnitPriceMinimum { get; set; }

        public double? UnitPriceMaximum { get; set; }

        [DisplayName("Expiration Date")]
        public DateTime? ExpirationDateMinimum { get; set; }

        public DateTime? ExpirationDateMaximum { get; set; }

        [DisplayName("Stores")]
        public List<int>? StoreIds { get; set; }
    }
}
