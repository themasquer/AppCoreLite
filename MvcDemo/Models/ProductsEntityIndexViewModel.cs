using AppCoreLite.ViewModels;
using DataAccessDemo.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcDemo.Models
{
    public class ProductsEntityIndexViewModel : PageOrderFilterViewModel
    {
        public List<ProductEntity> Products { get; set; }
        public SelectList Categories { get; set; }
        public MultiSelectList Stores { get; set; }
    }
}
