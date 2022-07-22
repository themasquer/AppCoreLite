using AppCoreLite.ViewModels;
using DataAccessDemo.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcDemo.Models
{
    public class ProductsIndexViewModel : PageOrderFilterViewModel
    {
        public List<ProductModel> Products { get; set; }
        public SelectList Categories { get; set; }
        public MultiSelectList Stores { get; set; }
    }
}
